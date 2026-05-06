using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Games.Media.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Media.Handlers
{
    public class RemovePictureToGameCommandHandler(IDatabase database, IS3Service s3Service, ILogger<RemovePictureToGameCommandHandler> logger)
        : ICommandHandler<RemoveStorePictureToGameCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveStorePictureToGameCommand command, CancellationToken cancellationToken)
        {
            var picture = await database.GamePictures
                .AsNoTracking()
                .Include(g => g.Game)
                .SingleOrDefaultAsync(g => g.Id == command.PictureId, cancellationToken);
            if (picture is null)
            {
                logger.LogWarning("Picture with id {PictureId} not found", command.PictureId);
                return Result.NotFound("Picture not found");
            }

            if (picture.Game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of the game with id {GameId}", command.IdentityId, picture.GameId);
                return Result.Unauthorized();
            }

            if (picture.ProcessingStatus != GamePictureProcessingStatus.Completed
                && picture.ProcessingStatus != GamePictureProcessingStatus.Failed)
            {
                logger.LogWarning("Cannot remove picture with id {PictureId} because it's not processed yet", command.PictureId);
                return Result.Error("Cannot remove a picture that is still being processed or pending processing");
            }

            var picturesCount = await database.GamePictures.CountAsync(g => g.GameId == picture.GameId, cancellationToken);
            if (picturesCount < 2)
            {
                logger.LogWarning("Cannot remove picture with id {PictureId} because it's the only picture of the game with id {GameId}", command.PictureId, picture.GameId);
                return Result.Error("Cannot remove the only picture of the game");
            }

            try
            {
                await s3Service.RemoveFileAsync($"{picture.OriginalRelativePath}/{picture.OriginalName}", cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while removing picture with id {PictureId}", command.PictureId);
                return Result.Error("An error occurred while removing the picture");
            }

            database.GamePictures.Remove(picture);
            await database.SaveChangesAsync(cancellationToken);
            return Result.NoContent();
        }
    }
}
