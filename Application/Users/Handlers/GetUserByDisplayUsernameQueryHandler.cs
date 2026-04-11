using Application.Contracts.Infrastructure;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetUserByDisplayUsernameQueryHandler(IDatabase database)
        : IQueryHandler<GetUserByDisplayUsernameQuery, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(GetUserByDisplayUsernameQuery query, CancellationToken cancellationToken)
        {
            var normalizedDisplayUsername = query.DisplayUsername.Trim().ToUpperInvariant();

            var user = await database.Users
                .AsNoTracking()
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
                .SingleOrDefaultAsync(u => u.NormalizedDisplayUsername == normalizedDisplayUsername, cancellationToken);

            if (user is null)
                return Result.NotFound();

            return Result.Success(ApplicationUser.FromEntity(user));
        }
    }
}