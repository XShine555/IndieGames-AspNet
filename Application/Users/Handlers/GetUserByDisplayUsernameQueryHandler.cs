using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Users.Handlers
{
    public class GetUserByDisplayUsernameQueryHandler(IDatabase database)
        : IQueryHandler<GetUsersQuery, PaginatedApplicationResponse<ApplicationUser> >
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationUser>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var normalizedDisplayUsername = query.DisplayUsername.Trim().ToUpperInvariant();

            var totalUsers = await database.Users.AsNoTracking()
                .CountAsync(cancellationToken);
            var pagedUsers = await database.Users.AsNoTracking()
                .Include(u => u.ProfilePicture)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.StorePictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.StorePictures)
                .Where(u => u.NormalizedDisplayUsername.Contains(normalizedDisplayUsername))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalUsers, cancellationToken);

            var applicationUsers = new List<ApplicationUser>(pagedUsers.Count);
            foreach (var user in pagedUsers)
            {
                applicationUsers.Add(ApplicationUser.FromEntity(user));
            }

            return new PaginatedApplicationResponse<ApplicationUser>(
                applicationUsers,
                pagedUsers.PageNumber,
                pagedUsers.PageSize,
                pagedUsers.PageCount,
                pagedUsers.TotalItemCount,
                pagedUsers.HasNextPage,
                pagedUsers.HasPreviousPage);
        }
    }
}