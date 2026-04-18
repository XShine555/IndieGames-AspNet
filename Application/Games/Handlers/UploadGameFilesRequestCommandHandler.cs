using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class UploadGameFilesRequestCommandHandler(IDatabase database, IS3Service s3Service, ILogger<UploadGameFilesRequestCommandHandler> logger)
        : ICommandHandler<UploadGameFilesRequestCommand, Result<ApplicationUploadGameFileRequestResponse>>
    {
        public async ValueTask<Result<ApplicationUploadGameFileRequestResponse>> Handle(UploadGameFilesRequestCommand command, CancellationToken cancellationToken)
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

            var storageKey = $"game-builds/{gameBuild.Id}/{Guid.NewGuid() }";
            var signedUrl = await s3Service.GetUploadUrlAsync(storageKey, TimeSpan.FromHours(1), cancellationToken);
            return Result.Success(new ApplicationUploadGameFileRequestResponse(
                storageKey,
                signedUrl));
        }
    }
}