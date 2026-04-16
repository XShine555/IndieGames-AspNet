using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class RemoveGameFromUserCartCommandHandler(
        IDatabase database,
        ILogger<RemoveGameFromUserCartCommandHandler> logger)
        : ICommandHandler<RemoveGameFromUserCartCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameFromUserCartCommand command, CancellationToken cancellationToken)
        {
            var cartItem = await database.UserCartItems
                .SingleOrDefaultAsync(ci => ci.UserId == command.UserId && ci.GameId == command.GameId, cancellationToken);
            if (cartItem is null)
            {
                logger.LogWarning("Cart item for user {UserId} and game {GameId} not found", command.UserId, command.GameId);
                return Result.NotFound("The game is not in the cart");
            }

            database.UserCartItems.Remove(cartItem);
            await database.SaveChangesAsync(cancellationToken);
            return Result.NoContent();
        }
    }
}
