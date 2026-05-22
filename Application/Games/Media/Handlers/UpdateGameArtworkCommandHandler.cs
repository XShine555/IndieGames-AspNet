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
    public class UpdateGameArtworkCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        IGameMediaMapper gameMediaMapper,
        GameConfiguration gameConfiguration,
        ILogger<UpdateGameArtworkCommandHandler> logger)
        : ICommandHandler<UpdateGameArtworkCommand, Result<ApplicationGameArtwork>>
    {
        public async ValueTask<Result<ApplicationGameArtwork>> Handle(UpdateGameArtworkCommand command, CancellationToken cancellationToken)
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
                    "User with id {IdentityId} is not the owner of game with id {GameId} and cannot update artwork {ArtworkId}",
                    command.IdentityId,
                    command.GameId,
                    command.ArtworkId);
                return Result.Forbidden();
            }

            var anyArtwork = await database.GameArtworks.AnyAsync(a => a.GameId == command.GameId && a.Id == command.ArtworkId && a.Type == artwork.Type
            && (
                a.ProcessingStatus == GameArtworkProcessingStatus.Processing
                || a.ProcessingStatus == GameArtworkProcessingStatus.Pending
            ), cancellationToken);

            if (anyArtwork)
            {
                logger.LogWarning(
                    "Cannot update artwork with id {ArtworkId} for game with id {GameId} because there is already an artwork of type {ArtworkType} being processed or pending",
                    command.ArtworkId,
                    command.GameId,
                    artwork.Type);
                return Result.Error("Cannot update artwork while another artwork of the same type is being processed or pending");
            }

            var sourceKey = BuildBucketKey(artwork.OriginalRelativePath, artwork.OriginalFileName);

            try
            {
                await s3Service.UploadFileAsync(command.FileData, sourceKey, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading updated artwork file for artwork id {ArtworkId}", artwork.Id);
                return Result.Error("Error uploading artwork file");
            }

            artwork.ProcessingStatus = GameArtworkProcessingStatus.Pending;
            artwork.ProcessingError = string.Empty;
            artwork.UpdatedAt = DateTime.UtcNow;

            await database.SaveChangesAsync(cancellationToken);

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
                return Result.Success(gameMediaMapper.ToApplicationGameArtwork(artwork));
            }
            catch (Exception exception)
            {
                artwork.ProcessingStatus = GameArtworkProcessingStatus.Failed;
                artwork.ProcessingError = "Failed to publish artwork processing event";
                artwork.UpdatedAt = DateTime.UtcNow;
                await database.SaveChangesAsync(cancellationToken);

                logger.LogError(exception, "Error publishing updated artwork event for artwork id {ArtworkId}", artwork.Id);
                return Result.Error("Error scheduling artwork processing");
            }
        }

        private static string BuildBucketKey(string relativePath, string fileName)
            => $"{relativePath}/{fileName}";
    }
}
