using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Configuration;
using Application.Games.Builds.Commands;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
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
                .SingleOrDefaultAsync(gb => gb.Id == command.BuildId, cancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Game build with ID {BuildId} not found", command.BuildId);
                return Result.NotFound();
            }

            if (gameBuild.Game.OwnerId != command.UserId)
            {
                logger.LogWarning("User {UserId} is not authorized to complete build {BuildId} for game {GameId}",
                    command.UserId,
                    command.BuildId,
                    gameBuild.GameId);
                return Result.Unauthorized();
            }

            if (gameBuild.Status == GameBuildStatus.Processing || gameBuild.Status == GameBuildStatus.PendingForProcessing)
            {
                logger.LogWarning("Game build {BuildId} is already scheduled for processing", command.BuildId);
                return Result.Conflict("Build is already being processed.");
            }

            if (gameBuild.Status == GameBuildStatus.Removing)
            {
                logger.LogWarning("Game build {BuildId} is being removed and cannot be completed", command.BuildId);
                return Result.Conflict("Build is being removed.");
            }

            gameBuild.Status = GameBuildStatus.PendingForProcessing;
            await database.SaveChangesAsync(cancellationToken);

            var buildStoragePath = gameConfiguration.Routes.BuildGameBuildPath(gameBuild.GameId, gameBuild.Id);
            var @event = new ProcessGameBuildFilesEvent(gameBuild.GameId, command.BuildId, buildStoragePath);

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
                    gameBuild.Id,
                    gameBuild.GameId);

                return Result.Error("Error scheduling build processing");
            }
        }
    }
}
