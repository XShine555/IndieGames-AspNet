using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class UpdateGameBuildCommandHandler(
        IDatabase database,
        IGameMapper gameMapper,
        ILogger<UpdateGameBuildCommandHandler> logger)
        : ICommandHandler<UpdateGameBuildCommand, Result<ApplicationGameBuildMutation>>
    {
        public async ValueTask<Result<ApplicationGameBuildMutation>> Handle(UpdateGameBuildCommand command, CancellationToken cancellationToken)
        {
            var gameBuild = await database.GameBuilds
                .Include(gb => gb.Game)
                .SingleOrDefaultAsync(gb => gb.Id == command.BuildId, cancellationToken);

            if (gameBuild is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found for update", command.BuildId);
                return Result.NotFound();
            }

            if (gameBuild.Game.OwnerId != command.UserId)
            {
                logger.LogWarning("User {UserId} is not authorized to update game build {BuildId}", command.UserId, command.BuildId);
                return Result.Unauthorized();
            }

            if (gameBuild.Status == GameBuildStatus.Completed)
            {
                logger.LogWarning("Game build {BuildId} cannot be updated because it is already completed", command.BuildId);
                return Result.Conflict("Completed builds cannot be updated.");
            }

            gameBuild.VersionName = command.VersionName.Trim();
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Game build {BuildId} updated by user {UserId}", command.BuildId, command.UserId);
            return Result.Success(gameMapper.ToApplicationGameBuildMutation(gameBuild));
        }
    }
}
