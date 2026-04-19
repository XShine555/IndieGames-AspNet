using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class UploadGameFilesRequestCommandHandler(IDatabase database, IS3Service s3Service,ILogger<UploadGameFilesRequestCommandHandler> logger,
        GameConfiguration gameConfiguration)
        : ICommandHandler<UploadGameFilesRequestCommand, Result<ApplicationUploadGameFilesRequestMutation>>
    {
        public async ValueTask<Result<ApplicationUploadGameFilesRequestMutation>> Handle(UploadGameFilesRequestCommand command, CancellationToken cancellationToken)
        {
            var gameBuild = await database.GameBuilds
                .Include(gb => gb.Game)
                .SingleOrDefaultAsync(gb => gb.Id == command.BuildId, cancellationToken);
            if (gameBuild is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found", command.BuildId);
                return Result.NotFound("Game build not found");
            }
            if (gameBuild.Game.OwnerId != command.UserId)
            {
                logger.LogWarning("User {UserId} is not the owner of game build {BuildId}", command.UserId, command.BuildId);
                return Result.Unauthorized();
            }

            var storageKey = gameConfiguration.Routes.BuildGameBuildPath(gameBuild.GameId, gameBuild.Id);
            var signedUrl = await s3Service.GetUploadUrlAsync(storageKey, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Generated signed URL for game build {BuildId} with storage key {StorageKey}", command.BuildId, storageKey);
            return Result.Success(new ApplicationUploadGameFilesRequestMutation(
                storageKey,
                signedUrl));
        }
    }
}