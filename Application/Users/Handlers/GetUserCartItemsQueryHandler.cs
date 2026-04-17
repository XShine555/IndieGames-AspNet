using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetUserCartItemsQueryHandler(
        IDatabase database,
        IUserMapper userMapper)
        : IQueryHandler<GetUserCartItemsQuery, Result<IReadOnlyCollection<ApplicationUserCartItem>>>
    {
        public async ValueTask<Result<IReadOnlyCollection<ApplicationUserCartItem>>> Handle(GetUserCartItemsQuery query, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == query.UserId, cancellationToken);
            if (!userExists)
            {
                return Result.NotFound();
            }

            var cartItems = await database.UserCartItems
                .AsNoTracking()
                .AsSplitQuery()
                .Where(ci => ci.UserId == query.UserId)
                .Include(ci => ci.Game)
                    .ThenInclude(g => g.Genres)
                .Include(ci => ci.Game)
                    .ThenInclude(g => g.StorePictures)
                .Include(ci => ci.Game)
                    .ThenInclude(g => g.Artworks)
                .OrderByDescending(ci => ci.AddedAt)
                .ToArrayAsync(cancellationToken);

            IReadOnlyCollection<ApplicationUserCartItem> mappedCartItems = cartItems
                .Select(userMapper.ToApplicationUserCartItem)
                .ToArray();

            return Result.Success(mappedCartItems);
        }
    }
}
