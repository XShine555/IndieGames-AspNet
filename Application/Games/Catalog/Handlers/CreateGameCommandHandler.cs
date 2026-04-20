using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Games.V1;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Configuration;
using Application.Games.Catalog.Commands;
using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Catalog.Handlers
{
    public class CreateGameCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus eventBus,
        IGameCatalogMapper gameCatalogMapper,
        GameConfiguration gameConfiguration,
        ILogger<CreateGameCommandHandler> logger)
        : ICommandHandler<CreateGameCommand, Result<ApplicationGameMutation>>
    {
        private record GameArtworkInput(GameArtworkType Type, IFileData FileData);

        public async ValueTask<Result<ApplicationGameMutation>> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var owner = await database.Users
                .SingleOrDefaultAsync(u => u.IdentityId == command.IdentityId, cancellationToken);
            if (owner is null)
            {
                logger.LogWarning("User with ID '{identityId}' not found.", command.IdentityId);
                return Result.NotFound("User not found.");
            }

            var normalizedTitle = command.Title.Trim().ToUpperInvariant();
            var existingGame = await database.Games
                .AsNoTracking()
                .AnyAsync(g => g.NormalizedTitle == normalizedTitle, cancellationToken);
            if (existingGame)
            {
                logger.LogWarning("A game with the title '{Title}' already exists.", command.Title);
                return Result.Conflict("A game with the same title already exists.");
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
            var inputArtworks = GetRequiredArtworks(command);

            try
            {
                foreach (var inputArtwork in inputArtworks)
                {
                    var originalName = Guid.NewGuid() + inputArtwork.FileData.FileExtension;
                    var originalRelativePath = gameConfiguration.Routes.GetOriginalArtworkFolderPath(game.Id, inputArtwork.Type);
                    var sourceKey = BuildBucketKey(originalRelativePath, originalName);

                    await s3Service.UploadFileAsync(inputArtwork.FileData, sourceKey, cancellationToken);
                    uploadedArtworkKeys.Add(sourceKey);

                    artworkRecords.Add(new GameArtwork
                    {
                        GameId = game.Id,
                        Type = inputArtwork.Type,
                        SortOrder = 0,
                        OriginalRelativePath = originalRelativePath,
                        OriginalFileName = originalName,
                        OriginalExtension = inputArtwork.FileData.FileExtension,
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
                var @event = new ProcessNewGameArtworkEvent(
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
                        "Error publishing artwork processing event for artwork id {ArtworkId}",
                        artwork.Id);
                }
            }

            if (anyPublishError)
                await database.SaveChangesAsync(cancellationToken);

            return Result.Created(gameCatalogMapper.ToApplicationGameMutation(game));
        }

        private static IReadOnlyCollection<GameArtworkInput> GetRequiredArtworks(CreateGameCommand command)
        {
            return
            [
                new GameArtworkInput(GameArtworkType.Capsule, command.CapsulePicture),
                new GameArtworkInput(GameArtworkType.Header, command.HeaderPicture),
                new GameArtworkInput(GameArtworkType.Main, command.MainPicture)
            ];
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
