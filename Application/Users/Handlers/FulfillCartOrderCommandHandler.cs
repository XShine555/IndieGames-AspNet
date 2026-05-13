using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class FulfillCartOrderCommandHandler(
        IDatabase database,
        ILogger<FulfillCartOrderCommandHandler> logger)
        : ICommandHandler<FulfillCartOrderCommand, Result>
    {
        public async ValueTask<Result> Handle(FulfillCartOrderCommand command, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User {UserId} not found for order fulfillment", command.UserId);
                return Result.NotFound("User not found");
            }

            foreach (var gameId in command.GameIds)
            {
                var alreadyOwned = await database.UserLibrary
                    .AsNoTracking()
                    .AnyAsync(ug => ug.UserId == command.UserId && ug.GameId == gameId, cancellationToken);

                if (!alreadyOwned)
                {
                    await database.UserLibrary.AddAsync(new UserOwnedGame
                    {
                        UserId = command.UserId,
                        GameId = gameId
                    }, cancellationToken);
                }

                var cartItem = await database.UserCartItems
                    .FirstOrDefaultAsync(ci => ci.UserId == command.UserId && ci.GameId == gameId, cancellationToken);

                if (cartItem is not null)
                    database.UserCartItems.Remove(cartItem);
            }

            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Order fulfilled for user {UserId}: {Count} games added to library", command.UserId, command.GameIds.Count);
            return Result.Success();
        }
    }
}
