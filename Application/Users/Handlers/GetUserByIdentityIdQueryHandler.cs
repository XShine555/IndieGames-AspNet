using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetUserByIdentityIdQueryHandler(IDatabase database)
        : IQueryHandler<GetUserByIdentityIdQuery, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(GetUserByIdentityIdQuery query, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .AsNoTracking()
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Pictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Pictures)
                .SingleOrDefaultAsync(u => u.IdentityId == query.IdentityId, cancellationToken);

            if (user is null)
                return Result.NotFound();

            return Result.Success(ApplicationUser.FromEntity(user));
        }
    }
}