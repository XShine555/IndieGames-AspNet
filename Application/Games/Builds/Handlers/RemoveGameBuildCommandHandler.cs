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
    public class RemoveGameBuildCommandHandler(
        IDatabase database,
        IEventBus eventBus,
        GameConfiguration gameConfiguration,
        ILogger<RemoveGameBuildCommandHandler> logger)
        : ICommandHandler<RemoveGameBuildCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameBuildCommand command, CancellationToken cancellationToken)
        {
            var gameBuild = await database.GameBuilds
                .Include(gb => gb.Game)
                .SingleOrDefaultAsync(gb => gb.Id == command.BuildId, cancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found for deletion", command.BuildId);
                return Result.NotFound("Game build not found");
            }

            if (gameBuild.Game.OwnerId != command.UserId)
            {
                logger.LogWarning("User {UserId} is not authorized to delete game build {BuildId}", command.UserId, command.BuildId);
                return Result.Unauthorized();
            }

            if (gameBuild.Game.ReleaseGameBuildId == gameBuild.Id)
            {
                logger.LogWarning("Game build {BuildId} cannot be deleted because it is the release build for game {GameId}", gameBuild.Id, gameBuild.GameId);
                return Result.Conflict("Cannot delete the current release build. Change the release build first.");
            }

            if (gameBuild.Status == GameBuildStatus.PendingForProcessing
                || gameBuild.Status == GameBuildStatus.Processing
                || gameBuild.Status == GameBuildStatus.Removing)
            {
                logger.LogWarning("Game build {BuildId} cannot be deleted because it is in status {Status}", gameBuild.Id, gameBuild.Status);
                return Result.Conflict("Build cannot be deleted while it is being processed.");
            }

            gameBuild.Status = GameBuildStatus.Removing;
            await database.SaveChangesAsync(cancellationToken);

            var buildStoragePath = gameConfiguration.Routes.BuildGameBuildPath(gameBuild.GameId, gameBuild.Id);
            var @event = new RemoveGameBuildEvent(gameBuild.GameId, gameBuild.Id, buildStoragePath);

            try
            {
                await eventBus.PublishAsync(@event, cancellationToken);
                logger.LogInformation("Game build {BuildId} marked for deletion by user {UserId}", command.BuildId, command.UserId);
                return Result.NoContent();
            }
            catch (Exception exception)
            {
                gameBuild.Status = GameBuildStatus.Failed;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception,
                    "Error scheduling build removal for build {BuildId} and game {GameId}",
                    gameBuild.Id,
                    gameBuild.GameId);

                return Result.Error("Error scheduling build deletion");
            }
        }
    }
}
