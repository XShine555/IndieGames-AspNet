using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class AddGameToUserCartCommandHandler(
        IDatabase database,
        ILogger<AddGameToUserCartCommandHandler> logger)
        : ICommandHandler<AddGameToUserCartCommand, Result>
    {
        private const int MaxCartItems = 15;

        public async ValueTask<Result> Handle(AddGameToUserCartCommand command, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var gameExists = await database.Games
                .AsNoTracking()
                .AnyAsync(g => g.Id == command.GameId, cancellationToken);
            if (!gameExists)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            var alreadyInCart = await database.UserCartItems
                .AsNoTracking()
                .AnyAsync(ci => ci.UserId == command.UserId && ci.GameId == command.GameId, cancellationToken);
            if (alreadyInCart)
            {
                logger.LogInformation("Game with id {GameId} is already in the cart of user {UserId}", command.GameId, command.UserId);
                return Result.Conflict("The game is already in the cart");
            }

            var cartItemsCount = await database.UserCartItems
                .AsNoTracking()
                .CountAsync(ci => ci.UserId == command.UserId, cancellationToken);
            if (cartItemsCount >= MaxCartItems)
            {
                logger.LogWarning("User with id {UserId} has reached the maximum number of cart items ({Max})", command.UserId, MaxCartItems);
                return Result.Invalid(new ValidationError($"The cart cannot contain more than {MaxCartItems} items"));
            }

            await database.UserCartItems.AddAsync(new UserCartItem
            {
                UserId = command.UserId,
                GameId = command.GameId
            }, cancellationToken);

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
