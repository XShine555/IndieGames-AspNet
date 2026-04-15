using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Users.Handlers
{
    public class GetUsersQueryHandler(IDatabase database)
        : IQueryHandler<GetUsersQuery, PaginatedApplicationResponse<ApplicationUserListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationUserListItem>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var normalizedDisplayName = query.DisplayName?.Trim().ToUpperInvariant();
            var hasDisplayNameFilter = !string.IsNullOrWhiteSpace(normalizedDisplayName);

            var baseQuery = database.Users
                .AsNoTracking()
                .AsQueryable();

            if (hasDisplayNameFilter)
            {
                baseQuery = baseQuery.Where(u => u.NormalizedDisplayUsername.Contains(normalizedDisplayName!));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var pageInfo = new StaticPagedList<Guid>(Array.Empty<Guid>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationUserListItem>(
                    Array.Empty<ApplicationUserListItem>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var users = await baseQuery
                .OrderBy(u => u.DisplayUsername)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => new ApplicationUserListItem(
                    u.IdentityId,
                    u.Username,
                    u.DisplayUsername,
                    u.ProfilePicture == null
                        ? null
                        : new ApplicationUserPicture(
                            u.ProfilePicture.Id,
                            (string.IsNullOrWhiteSpace(u.ProfilePicture.OriginalRelativePath) || string.IsNullOrWhiteSpace(u.ProfilePicture.OriginalName))
                                ? string.Empty
                                : $"{u.ProfilePicture.OriginalRelativePath}/{u.ProfilePicture.OriginalName}",
                            (string.IsNullOrWhiteSpace(u.ProfilePicture.SmallRelativePath) || string.IsNullOrWhiteSpace(u.ProfilePicture.SmallName))
                                ? string.Empty
                                : $"{u.ProfilePicture.SmallRelativePath}/{u.ProfilePicture.SmallName}",
                            (string.IsNullOrWhiteSpace(u.ProfilePicture.MediumRelativePath) || string.IsNullOrWhiteSpace(u.ProfilePicture.MediumName))
                                ? string.Empty
                                : $"{u.ProfilePicture.MediumRelativePath}/{u.ProfilePicture.MediumName}",
                            (string.IsNullOrWhiteSpace(u.ProfilePicture.LargeRelativePath) || string.IsNullOrWhiteSpace(u.ProfilePicture.LargeName))
                                ? string.Empty
                                : $"{u.ProfilePicture.LargeRelativePath}/{u.ProfilePicture.LargeName}",
                            u.ProfilePicture.AddedAt),
                    u.CreatedGames.Count,
                    u.OwnedGames.Count,
                    u.CreatedAt,
                    u.UpdatedAt))
                .ToArrayAsync(cancellationToken);

            return new PaginatedApplicationResponse<ApplicationUserListItem>(
                users,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}
