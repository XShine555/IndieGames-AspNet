using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Configuration;
using Application.Games.Media.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Media.Handlers
{
    public class RetryGameStorePictureCommandHandler(
        IDatabase database,
        IEventBus eventBus,
        GameConfiguration gameConfiguration,
        ILogger<RetryGameStorePictureCommandHandler> logger)
        : ICommandHandler<RetryGameStorePictureCommand, Result>
    {
        public async ValueTask<Result> Handle(RetryGameStorePictureCommand command, CancellationToken cancellationToken)
        {
            var picture = await database.GamePictures
                .Include(p => p.Game)
                .SingleOrDefaultAsync(p => p.Id == command.PictureId && p.GameId == command.GameId, cancellationToken);

            if (picture is null)
            {
                logger.LogWarning("Store picture with id {PictureId} not found for game {GameId}", command.PictureId, command.GameId);
                return Result.NotFound();
            }

            if (picture.Game.OwnerId != command.IdentityId)
            {
                logger.LogWarning(
                    "User with id {IdentityId} is not the owner of game with id {GameId} and cannot retry store picture {PictureId}",
                    command.IdentityId,
                    command.GameId,
                    command.PictureId);
                return Result.Forbidden();
            }

            if (picture.ProcessingStatus == GamePictureProcessingStatus.Processing)
            {
                logger.LogWarning("Store picture with id {PictureId} is already processing and cannot be retried", picture.Id);
                return Result.Conflict("Store picture is already processing");
            }

            picture.ProcessingStatus = GamePictureProcessingStatus.Pending;
            picture.Game.StoreReadinessStatus = GameStoreReadinessStatus.NotReadyForStore;

            await database.SaveChangesAsync(cancellationToken);

            var sourceKey = BuildBucketKey(picture.OriginalRelativePath, picture.OriginalName);
            var @event = new GenerateGamesPicturesEvent(
                picture.Id,
                sourceKey,
                gameConfiguration.Routes.GetSmallPictureFolderPath(picture.GameId),
                gameConfiguration.Routes.GetMediumPictureFolderPath(picture.GameId),
                gameConfiguration.Routes.GetLargePictureFolderPath(picture.GameId),
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
                picture.ProcessingStatus = GamePictureProcessingStatus.Failed;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception, "Error publishing retry event for store picture id {PictureId}", picture.Id);
                return Result.Error("Error scheduling store picture retry");
            }
        }

        private static string BuildBucketKey(string relativePath, string fileName)
            => $"{relativePath}/{fileName}";
    }
}
