using Application.Abstractions.Persistence;
using Application.Games.Catalog.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Games.Catalog.Handlers
{
    public class RemoveGameCommandHandler(IDatabase database, ILogger<RemoveGameCommandHandler> logger)
        : ICommandHandler<RemoveGameCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.Id, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for deletion", command.Id);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("Unauthorized deletion attempt for game with id {GameId} by user {UserId}", command.Id, command.IdentityId);
                return Result.Unauthorized();
            }

            database.Games.Remove(game);
            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Game with id {GameId} successfully deleted by user {UserId}", command.Id, command.IdentityId);
            return Result.NoContent();
        }
    }
}
