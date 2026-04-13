using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class CreateGameCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        GameConfiguration gameConfiguration,
        ILogger<CreateGameCommandHandler> logger)
        : ICommandHandler<CreateGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var owner = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.IdentityId == command.identityId, cancellationToken);
            if (owner is null)
            {
                logger.LogWarning("User with ID '{identityId}' not found.", command.identityId);
                return Result.NotFound();
            }

            var artworkValidationError = ValidateArtworks(command.Artworks);
            if (!string.IsNullOrWhiteSpace(artworkValidationError))
                return Result.Invalid(new ValidationError(artworkValidationError));

            var normalizedTitle = command.Title.Trim().ToUpperInvariant();
            var existingGame = await database.Games
                .AsNoTracking()
                .AnyAsync(g => g.Title.ToUpper() == normalizedTitle, cancellationToken);
            if (existingGame)
            {
                logger.LogWarning("A game with the title '{Title}' already exists.", command.Title);
                return Result.Conflict();
            }

            var genres = await database.Genres
                .Where(g => command.Genres.Contains(g.Id))
                .ToListAsync(cancellationToken);
            if (genres.Count != command.Genres.Count)
            {
                logger.LogWarning("One or more genres not found for IDs: {GenreIds}.", string.Join(", ", command.Genres));
                return Result.Invalid(new ValidationError("One or more genres not found."));
            }

            var game = CreateGameCommand.ToEntity(command, genres);
            game.Owner = owner;

            await database.Games.AddAsync(game, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var uploadedArtworkKeys = new List<string>();
            var artworkRecords = new List<GameArtwork>();
            try
            {
                foreach (var artwork in command.Artworks)
                {
                    var originalName = Guid.NewGuid() + artwork.FileData.FileExtension;
                    var originalRelativePath = gameConfiguration.Routes.GetOriginalArtworkFolderPath(game.Id, artwork.Type);
                    var sourceKey = BuildBucketKey(originalRelativePath, originalName);

                    await s3Service.UploadFileAsync(artwork.FileData, sourceKey, cancellationToken);
                    uploadedArtworkKeys.Add(sourceKey);

                    artworkRecords.Add(new GameArtwork
                    {
                        GameId = game.Id,
                        Type = artwork.Type,
                        SortOrder = 0,
                        OriginalRelativePath = originalRelativePath,
                        OriginalFileName = originalName,
                        OriginalExtension = artwork.FileData.FileExtension,
                        ProcessingStatus = GameArtworkProcessingStatus.Pending,
                        ProcessingError = string.Empty
                    });
                }

                await database.GameArtworks.AddRangeAsync(artworkRecords, cancellationToken);
                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while creating artwork records for game {GameId}", game.Id);
                await CleanupUploadedFilesAsync(uploadedArtworkKeys, cancellationToken);
                database.Games.Remove(game);
                await database.SaveChangesAsync(cancellationToken);
                return Result.Error("Error creating game artworks");
            }

            var anyPublishError = false;
            foreach (var artwork in artworkRecords)
            {
                var sourceKey = BuildBucketKey(artwork.OriginalRelativePath, artwork.OriginalFileName);
                var @event = new GenerateGameArtworksEvent(
                    artwork.Id,
                    sourceKey,
                    gameConfiguration.Routes.GetSmallArtworkFolderPath(game.Id, artwork.Type),
                    gameConfiguration.Routes.GetMediumArtworkFolderPath(game.Id, artwork.Type),
                    gameConfiguration.Routes.GetLargeArtworkFolderPath(game.Id, artwork.Type),
                    new PictureResizeSize(gameConfiguration.ArtworkSizes.Small.Width, gameConfiguration.ArtworkSizes.Small.Height),
                    new PictureResizeSize(gameConfiguration.ArtworkSizes.Medium.Width, gameConfiguration.ArtworkSizes.Medium.Height),
                    new PictureResizeSize(gameConfiguration.ArtworkSizes.Large.Width, gameConfiguration.ArtworkSizes.Large.Height));

                try
                {
                    await eventBus.PublishAsync(@event, cancellationToken);
                }
                catch (Exception exception)
                {
                    anyPublishError = true;
                    artwork.ProcessingStatus = GameArtworkProcessingStatus.Failed;
                    artwork.ProcessingError = "Failed to publish artwork processing event";
                    logger.LogError(exception,
                        "Error publishing artwork generation event for artwork id {ArtworkId}",
                        artwork.Id);
                }
            }

            if (anyPublishError)
                await database.SaveChangesAsync(cancellationToken);

            return Result.Created(ApplicationGame.FromEntity(game));
        }

        private static string ValidateArtworks(ICollection<CreateGameArtworkInput> artworks)
        {
            if (artworks.Count == 0)
                return "Artworks are required.";

            var requiredArtworkTypes = Enum.GetValues<GameArtworkType>();
            foreach (var artworkType in requiredArtworkTypes)
            {
                var count = artworks.Count(x => x.Type == artworkType);
                if (count == 0)
                    return $"Missing artwork for type {artworkType}.";

                if (count > 1)
                    return $"Only one artwork is allowed for type {artworkType}.";
            }

            if (artworks.Count != requiredArtworkTypes.Length)
                return "Unexpected artwork count for game creation.";

            if (artworks.Any(x => x.FileData == null))
                return "Artwork file data is required.";

            return string.Empty;
        }

        private async Task CleanupUploadedFilesAsync(IReadOnlyCollection<string> uploadedKeys, CancellationToken cancellationToken)
        {
            foreach (var uploadedKey in uploadedKeys)
            {
                try
                {
                    await s3Service.RemoveFileAsync(uploadedKey, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception, "Error cleaning up uploaded artwork file {UploadedKey}", uploadedKey);
                }
            }
        }

        private static string BuildBucketKey(string route, string fileName)
        {
            return $"{route}/{fileName}";
        }
    }
}
