using Application.Abstractions.Persistence;
using Application.Games.Builds.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class RemoveGameBuildCommandHandler(
        IDatabase database,
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
                return Result.NotFound();
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

            database.GameBuilds.Remove(gameBuild);
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Game build {BuildId} deleted by user {UserId}", command.BuildId, command.UserId);
            return Result.NoContent();
        }
    }
}
