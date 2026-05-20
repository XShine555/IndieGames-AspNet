using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Media.Commands;
using Application.Games.Media.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Media.Handlers
{
    public class AddStorePictureToGameCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        IGameMediaMapper gameMediaMapper,
        ILogger<AddStorePictureToGameCommandHandler> logger,
        GameConfiguration gameConfiguration)
        : ICommandHandler<AddStorePictureToGameCommand, Result<ApplicationGamePicture>>
    {
        public async ValueTask<Result<ApplicationGamePicture>> Handle(AddStorePictureToGameCommand command, CancellationToken cancellationToken)
        {
            var gameResult = await ValidateGameOwnerAsync(command, cancellationToken);
            if (!gameResult.IsSuccess)
            {
                return gameResult.Status switch
                {
                    ResultStatus.NotFound => Result.NotFound("Game not found"),
                    ResultStatus.Unauthorized => Result.Unauthorized(),
                    _ => Result.Error()
                };
            }

            var game = gameResult.Value;

            var pictureName = Guid.NewGuid() + command.fileData.FileExtension;
            var pictureKey = gameConfiguration.Routes.BuildStorePicturePath(game.Id, pictureName);

            var uploadResult = await UploadOriginalPictureAsync(command, pictureKey, cancellationToken);
            if (!uploadResult.IsSuccess)
                return Result.Error("Error uploading picture");

            var pictureResult = await CreatePictureRecordAsync(command, pictureName, cancellationToken);
            if (!pictureResult.IsSuccess)
            {
                await s3Service.RemoveFileAsync(pictureKey, cancellationToken);
                return Result.Error("Error saving picture");
            }

            var newPicture = pictureResult.Value;
            var enqueueResult = await PublishGeneratePicturesEventAsync(game.Id, newPicture.Id, pictureKey, cancellationToken);
            if (!enqueueResult.IsSuccess)
            {
                await RemovePictureRecordAsync(newPicture, cancellationToken);
                await s3Service.RemoveFileAsync(pictureKey, cancellationToken);
                return Result.Error("Error scheduling picture processing");
            }

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success(gameMediaMapper.ToApplicationGamePicture(newPicture));
        }

        private async Task<Result<Game>> ValidateGameOwnerAsync(AddStorePictureToGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.AsNoTracking()
                .Include(x => x.Owner)
                .Include(x => x.StorePictures)
                .Include(x => x.Genres)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId}", command.IdentityId, command.GameId);
                return Result.Unauthorized();
            }

            return Result.Success(game);
        }

        private async Task<Result> UploadOriginalPictureAsync(AddStorePictureToGameCommand command, string pictureKey, CancellationToken cancellationToken)
        {
            try
            {
                await s3Service.UploadFileAsync(command.fileData, pictureKey, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading picture for game with id {GameId}", command.GameId);
                return Result.Error();
            }
        }

        private async Task<Result<GameStorePictures>> CreatePictureRecordAsync(AddStorePictureToGameCommand command, string pictureName, CancellationToken cancellationToken)
        {
            var newPicture = new GameStorePictures
            {
                GameId = command.GameId,
                OriginalName = pictureName,
                OriginalContentType = command.fileData.ContentType,
                OriginalRelativePath = gameConfiguration.Routes.GetStorePictureFolderPath(command.GameId),
                ProcessingStatus = GamePictureProcessingStatus.Pending
            };

            await database.GamePictures.AddAsync(newPicture, cancellationToken);
            try
            {
                await database.SaveChangesAsync(cancellationToken);
                return Result.Success(newPicture);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error saving picture for game with id {GameId}", command.GameId);
                return Result.Error();
            }
        }

        private async Task<Result> PublishGeneratePicturesEventAsync(Guid gameId, Guid pictureId, string sourceKey, CancellationToken cancellationToken)
        {
            var @event = new GenerateGamesPicturesEvent(
                pictureId,
                sourceKey,
                gameConfiguration.Routes.GetSmallPictureFolderPath(gameId),
                gameConfiguration.Routes.GetMediumPictureFolderPath(gameId),
                gameConfiguration.Routes.GetLargePictureFolderPath(gameId),
                new PictureResizeSize(gameConfiguration.Sizes.Small.Width, gameConfiguration.Sizes.Small.Height),
                new PictureResizeSize(gameConfiguration.Sizes.Medium.Width, gameConfiguration.Sizes.Medium.Height),
                new PictureResizeSize(gameConfiguration.Sizes.Large.Width, gameConfiguration.Sizes.Large.Height));

            try
            {
                await eventBus.PublishAsync(@event, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error publishing picture generation event for picture id {PictureId}", pictureId);
                return Result.Error();
            }
        }

        private async Task RemovePictureRecordAsync(GameStorePictures picture, CancellationToken cancellationToken)
        {
            database.GamePictures.Remove(picture);
            await database.SaveChangesAsync(cancellationToken);
        }
    }
}
