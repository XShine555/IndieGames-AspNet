using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Configuration;
using Application.Games.Commands;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class CompleteGameBuildCommandHandler(
        IDatabase database,
        IEventBus eventBus,
        GameConfiguration gameConfiguration,
        ILogger<CompleteGameBuildCommandHandler> logger)
        : ICommandHandler<CompleteGameBuildCommand, Result>
    {
        public async ValueTask<Result> Handle(CompleteGameBuildCommand command, CancellationToken cancellationToken)
        {
            var gameBuild = await database.GameBuilds
                .Include(gb => gb.Game)
                .SingleOrDefaultAsync(gb => gb.Id == command.BuildId && gb.GameId == command.GameId, cancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Game build with ID {BuildId} for game {GameId} not found", command.BuildId, command.GameId);
                return Result.NotFound();
            }

            if (gameBuild.Game.OwnerId != command.UserId)
            {
                logger.LogWarning("User with ID {UserId} is not the owner of game with ID {GameId}", command.UserId, command.GameId);
                return Result.Unauthorized();
            }

            if (gameBuild.Status == GameBuildStatus.Processing || gameBuild.Status == GameBuildStatus.PendingForProcessing)
            {
                logger.LogWarning("Game build {BuildId} is already scheduled for processing", command.BuildId);
                return Result.Conflict("Build is already being processed.");
            }

            gameBuild.Status = GameBuildStatus.PendingForProcessing;
            await database.SaveChangesAsync(cancellationToken);

            var buildStoragePath = gameConfiguration.Routes.BuildGameBuildPath(command.GameId, command.BuildId);
            var @event = new ProcessGameBuildFilesEvent(command.GameId, command.BuildId, buildStoragePath);

            try
            {
                await eventBus.PublishAsync(@event, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                gameBuild.Status = GameBuildStatus.Failed;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception,
                    "Error scheduling build processing for build {BuildId} and game {GameId}",
                    command.BuildId,
                    command.GameId);

                return Result.Error("Error scheduling build processing");
            }
        }
    }
}