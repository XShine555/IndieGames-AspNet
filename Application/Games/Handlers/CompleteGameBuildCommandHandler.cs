using Application.Abstractions.Persistence;
using Application.Games.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class CompleteGameBuildCommandHandler(IDatabase database, ILogger<CompleteGameBuildCommandHandler> logger)
        : ICommandHandler<CompleteGameBuildCommand, Result>
    {
        public async ValueTask<Result> Handle(CompleteGameBuildCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with ID {GameId} not found", command.GameId);
                return Result.NotFound();
            }
            if (game.OwnerId != command.UserId)
            {
                logger.LogWarning("User with ID {UserId} is not the owner of game with ID {GameId}", command.UserId, command.GameId);
                return Result.Unauthorized();
            }

            return Result.Success();
        }
    }
}