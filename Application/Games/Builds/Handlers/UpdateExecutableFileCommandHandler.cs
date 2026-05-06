using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Builds.Commands;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class UpdateExecutableFileCommandHandler(IDatabase database, IS3Service s3Service,
        ILogger<UpdateExecutableFileCommandHandler> logger, GameConfiguration gameConfiguration, IGameBuildMapper gameBuildMapper)
        : ICommandHandler<UpdateExecutableFileCommand, Result<ApplicationGameBuildMutation>>
    {
        public async ValueTask<Result<ApplicationGameBuildMutation>> Handle(UpdateExecutableFileCommand command, CancellationToken cancellationToken)
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
                logger.LogWarning("User with id {UserId} is not the owner of the game build with id {BuildId}", command.UserId, command.BuildId);
                return Result.Unauthorized();
            }

            if (gameBuild.Status != GameBuildStatus.UploadingFiles)
            {
                logger.LogWarning("Game build with id {BuildId} is not in uploading files status", command.BuildId);
                return Result.Invalid(new ValidationError("Game build is not in uploading files status"));
            }

            var key = gameConfiguration.Routes.BuildGameBuildFilePath(gameBuild.GameId, gameBuild.Id, command.FileKey);
            S3FileData fileData;
            try
            {
                fileData = await s3Service.GetFileDataAsync(key, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "File with key {FileKey} not found in storage for game build with id {BuildId}", command.FileKey, command.BuildId);
                return Result.NotFound("File not found in storage");
            }

            gameBuild.ExecutableFileName = command.FileKey;
            gameBuild.ExecutableRelativePath = Path.GetDirectoryName(command.FileKey);
            gameBuild.ExecutableContentType = fileData.ContentType;
            return Result.Success(gameBuildMapper.ToApplicationGameBuildMutation(gameBuild));
        }
    }
}