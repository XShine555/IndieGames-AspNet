using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Users.Handlers
{
    public class GetUsersQueryHandler(
        IDatabase database,
        IUserMapper userMapper)
        : IQueryHandler<GetUsersQuery, PaginatedApplicationResponse<ApplicationUser>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationUser>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
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
            var pageInfo = new StaticPagedList<string>(Array.Empty<string>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationUser>(
                    Array.Empty<ApplicationUser>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var pagedUserIds = await baseQuery
                .OrderBy(u => u.DisplayUsername)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(u => u.IdentityId)
                .ToListAsync(cancellationToken);

            var users = await database.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Include(u => u.ProfilePicture)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Owner)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.StorePictures)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Artworks)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Owner)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.StorePictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Artworks)
                .Where(u => pagedUserIds.Contains(u.IdentityId))
                .ToListAsync(cancellationToken);

            var usersById = users.ToDictionary(user => user.IdentityId);
            var orderedUsers = pagedUserIds
                .Select(userId => usersById[userId])
                .Select(userMapper.ToApplicationUser)
                .ToArray();

            return new PaginatedApplicationResponse<ApplicationUser>(
                orderedUsers,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}
