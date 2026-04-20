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
    public class RetryGameArtworkCommandHandler(
        IDatabase database,
        IEventBus eventBus,
        GameConfiguration gameConfiguration,
        ILogger<RetryGameArtworkCommandHandler> logger)
        : ICommandHandler<RetryGameArtworkCommand, Result>
    {
        public async ValueTask<Result> Handle(RetryGameArtworkCommand command, CancellationToken cancellationToken)
        {
            var artwork = await database.GameArtworks
                .Include(a => a.Game)
                .SingleOrDefaultAsync(a => a.Id == command.ArtworkId && a.GameId == command.GameId, cancellationToken);

            if (artwork is null)
            {
                logger.LogWarning("Artwork with id {ArtworkId} not found for game {GameId}", command.ArtworkId, command.GameId);
                return Result.NotFound();
            }

            if (artwork.Game.OwnerId != command.IdentityId)
            {
                logger.LogWarning(
                    "User with id {IdentityId} is not the owner of game with id {GameId} and cannot retry artwork {ArtworkId}",
                    command.IdentityId,
                    command.GameId,
                    command.ArtworkId);
                return Result.Forbidden();
            }

            if (artwork.ProcessingStatus == GameArtworkProcessingStatus.Processing)
            {
                logger.LogWarning("Artwork with id {ArtworkId} is already processing and cannot be retried", artwork.Id);
                return Result.Conflict("Artwork is already processing");
            }

            artwork.ProcessingStatus = GameArtworkProcessingStatus.Pending;
            artwork.ProcessingError = string.Empty;
            artwork.UpdatedAt = DateTime.UtcNow;
            artwork.Game.StoreReadinessStatus = GameStoreReadinessStatus.NotReadyForStore;

            await database.SaveChangesAsync(cancellationToken);

            var sourceKey = BuildBucketKey(artwork.OriginalRelativePath, artwork.OriginalFileName);
            var @event = new ProcessNewGameArtworkEvent(
                artwork.Id,
                sourceKey,
                gameConfiguration.Routes.GetSmallArtworkFolderPath(artwork.GameId, artwork.Type),
                gameConfiguration.Routes.GetMediumArtworkFolderPath(artwork.GameId, artwork.Type),
                gameConfiguration.Routes.GetLargeArtworkFolderPath(artwork.GameId, artwork.Type),
                new PictureResizeSize(gameConfiguration.ArtworkSizes.Small.Width, gameConfiguration.ArtworkSizes.Small.Height),
                new PictureResizeSize(gameConfiguration.ArtworkSizes.Medium.Width, gameConfiguration.ArtworkSizes.Medium.Height),
                new PictureResizeSize(gameConfiguration.ArtworkSizes.Large.Width, gameConfiguration.ArtworkSizes.Large.Height));

            try
            {
                await eventBus.PublishAsync(@event, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                artwork.ProcessingStatus = GameArtworkProcessingStatus.Failed;
                artwork.ProcessingError = "Failed to publish artwork processing event";
                artwork.UpdatedAt = DateTime.UtcNow;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception, "Error publishing retry event for artwork id {ArtworkId}", artwork.Id);
                return Result.Error("Error scheduling artwork retry");
            }
        }

        private static string BuildBucketKey(string relativePath, string fileName)
            => $"{relativePath}/{fileName}";
    }
}
