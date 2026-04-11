using Application.Configuration;
using Application.Abstractions;
using Application.Events;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class AddStorePictureToGameCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        ILogger<AddStorePictureToGameCommandHandler> logger,
        GameConfiguration gameConfiguration)
        : ICommandHandler<AddStorePictureToGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(AddStorePictureToGameCommand command, CancellationToken cancellationToken)
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
            {
                return Result.Error("Error uploading picture");
            }

            var newPicture = await CreatePictureRecordAsync(command, pictureName, cancellationToken);
            if (newPicture is null)
            {
                await s3Service.RemoveFileAsync(pictureKey, cancellationToken);
                return Result.Error("Error saving picture");
            }

            var enqueueResult = await PublishGeneratePicturesEventAsync(game.Id, newPicture.Id, pictureKey, cancellationToken);
            if (!enqueueResult.IsSuccess)
            {
                database.GamePictures.Remove(newPicture);
                await database.SaveChangesAsync(cancellationToken);
                await s3Service.RemoveFileAsync(pictureKey, cancellationToken);
                return Result.Error("Error scheduling picture processing");
            }

            return Result.Success(ApplicationGame.FromEntity(game));
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

        private async Task<GameStorePictures?> CreatePictureRecordAsync(AddStorePictureToGameCommand command, string pictureName, CancellationToken cancellationToken)
        {
            var newPicture = new GameStorePictures
            {
                GameId = command.GameId,
                OriginalName = pictureName,
                FileExtension = command.fileData.FileExtension,
                Name = pictureName,
                RelativePath = gameConfiguration.Routes.GetStorePictureFolderPath(command.GameId),
                ProcessingStatus = GamePictureProcessingStatus.Pending
            };

            await database.GamePictures.AddAsync(newPicture, cancellationToken);
            try
            {
                await database.SaveChangesAsync(cancellationToken);
                return newPicture;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error saving picture for game with id {GameId}", command.GameId);
                return null;
            }
        }

        private async Task<Result> PublishGeneratePicturesEventAsync(int gameId, int pictureId, string sourceKey, CancellationToken cancellationToken)
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
    }
}