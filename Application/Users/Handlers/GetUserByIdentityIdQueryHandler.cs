using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetUserByIdentityIdQueryHandler(
        IDatabase database,
        IUserMapper userMapper)
        : IQueryHandler<GetUserByIdentityIdQuery, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(GetUserByIdentityIdQuery query, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .AsNoTracking()
                .AsSplitQuery()
                .Include(u => u.ProfilePicture)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.StorePictures)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Artworks)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.StorePictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Artworks)
                .Include(u => u.CartItems)
                    .ThenInclude(ci => ci.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.CartItems)
                    .ThenInclude(ci => ci.Game)
                        .ThenInclude(g => g.StorePictures)
                .Include(u => u.CartItems)
                    .ThenInclude(ci => ci.Game)
                        .ThenInclude(g => g.Artworks)
                .SingleOrDefaultAsync(u => u.IdentityId == query.IdentityId, cancellationToken);

            if (user is null)
                return Result.NotFound();

            return Result.Success(userMapper.ToApplicationUser(user));
        }
    }
}
