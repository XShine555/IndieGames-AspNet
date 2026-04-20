using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Builds.Commands;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class PreSignGameFilesRequestCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        ILogger<PreSignGameFilesRequestCommandHandler> logger,
        GameConfiguration gameConfiguration)
        : ICommandHandler<PreSignGameFilesRequestCommand, Result<IReadOnlyList<ApplicationPreSignGameFileRequestMutation>>>
    {
        public async ValueTask<Result<IReadOnlyList<ApplicationPreSignGameFileRequestMutation>>> Handle(PreSignGameFilesRequestCommand command, CancellationToken cancellationToken)
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

            var mutations = new List<ApplicationPreSignGameFileRequestMutation>();
            foreach (var filePath in command.FilePaths)
            {
                try
                {
                    var storageKey = gameConfiguration.Routes.BuildGameBuildFilePath(gameBuild.GameId, gameBuild.Id, filePath);
                    var signedUrl = await s3Service.GetUploadUrlAsync(storageKey, TimeSpan.FromHours(2), cancellationToken);
                    mutations.Add(new ApplicationPreSignGameFileRequestMutation(
                        filePath,
                        storageKey,
                        signedUrl));
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Error generating signed URL for file {FilePath} in game build {BuildId}", filePath, command.BuildId);
                    return Result.Error("An error occurred while generating signed URLs for the game files");
                }
            }

            logger.LogInformation("Generated {Count} signed URLs for game build {BuildId}", mutations.Count, command.BuildId);
            return Result.Success((IReadOnlyList<ApplicationPreSignGameFileRequestMutation>)mutations);
        }
    }
}
