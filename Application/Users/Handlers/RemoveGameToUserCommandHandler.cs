using Application.Contracts.Infrastructure;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class RemoveGameToUserCommandHandler(IDatabase database, ILogger<RemoveGameToUserCommandHandler> logger)
        : ICommandHandler<RemoveGameToUserCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameToUserCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users.FindAsync(command.UserId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var game = await database.Games.FindAsync(command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            var ownedGame = await database.UserOwnedGames.FindAsync(
                [ command.UserId, command.GameId],
                cancellationToken);
            if (ownedGame is null)
            {
                logger.LogWarning("User with id {UserId} does not own game with id {GameId}", command.UserId, command.GameId);
                return Result.NotFound("User does not own this game");
            }

            database.UserOwnedGames.Remove(ownedGame);
            await database.SaveChangesAsync(cancellationToken);
            return Result.NoContent();
        }
    }
}