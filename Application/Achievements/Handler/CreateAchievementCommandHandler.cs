using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Achievements.V1;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Achievements.Commands;
using Application.Achievements.Responses;
using Ardalis.Result;
using Domain.Entities;
using AchievementEntity = Domain.Entities.Achievements;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class CreateAchievementCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus bus,
        IAchievementMapper achievementMapper,
        ILogger<CreateAchievementCommandHandler> logger)
        : ICommandHandler<CreateAchievementCommand, Result<ApplicationAchievement>>
    {
        private const string AchievementsFolderName = "achievements";
        private const string PicturesFolderName = "Pictures";
        private const string SmallPicturesFolderName = "SmallPictures";
        private const string MediumPicturesFolderName = "MediumPictures";
        private const string LargePicturesFolderName = "LargePictures";

        public async ValueTask<Result<ApplicationAchievement>> Handle(CreateAchievementCommand command, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            var user = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with ID {UserId} not found while creating achievement for game {GameId}", command.UserId, command.GameId);
                return Result.NotFound("User not found.");
            }

            var game = await database.Games
                .AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with ID {GameId} not found while creating achievement", command.GameId);
                return Result.NotFound("Game not found.");
            }

            if (game.OwnerId != user.IdentityId)
            {
                logger.LogWarning(
                    "User with ID {UserId} attempted to create an achievement for game {GameId} without owning it",
                    command.UserId,
                    command.GameId);
                return Result.Unauthorized("Only the game owner can create achievements.");
            }

            var normalizedName = NormalizeName(command.Name);
            var achievementAlreadyExists = await database.Achievements
                .AsNoTracking()
                .AnyAsync(a => a.GameId == command.GameId && a.NormalizedName == normalizedName, cancellationToken);
            if (achievementAlreadyExists)
            {
                logger.LogWarning(
                    "An achievement with the normalized name {NormalizedName} already exists for game {GameId}",
                    normalizedName,
                    command.GameId);
                return Result.Conflict("An achievement with the same name already exists.");
            }

            var achievement = new AchievementEntity
            {
                Id = Guid.NewGuid(),
                GameId = command.GameId,
                Name = command.Name.Trim(),
                NormalizedName = normalizedName,
                Description = command.Description.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var pictureName = Guid.NewGuid() + command.Picture.FileExtension;
            var sourceKey = BuildBucketKey(BuildAchievementPictureFolderPath(command.GameId, achievement.Id, PicturesFolderName), pictureName);

            var uploadResult = await UploadOriginalPictureAsync(command.Picture, sourceKey, cancellationToken);
            if (!uploadResult.IsSuccess)
                return Result.Error("Error uploading achievement picture");

            await database.Achievements.AddAsync(achievement, cancellationToken);
            try
            {
                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error saving achievement metadata for game {GameId} and user {UserId}", command.GameId, command.UserId);
                await s3Service.RemoveFileAsync(sourceKey, cancellationToken);
                return Result.Error("Error saving achievement");
            }

            var publishResult = await PublishAchievementPicturesEventAsync(command, achievement, pictureName, sourceKey, cancellationToken);
            if (!publishResult.IsSuccess)
            {
                await RemoveUploadedPictureAsync(sourceKey, cancellationToken);
                await RemovePersistedAchievementAsync(achievement, cancellationToken);
                return Result.Error("Error scheduling achievement picture processing");
            }

            return Result.Created(achievementMapper.ToApplicationAchievement(achievement));
        }

        private async Task<Result> UploadOriginalPictureAsync(IFileData picture, string pictureKey, CancellationToken cancellationToken)
        {
            try
            {
                await s3Service.UploadFileAsync(picture, pictureKey, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading achievement picture to {PictureKey}", pictureKey);
                return Result.Error();
            }
        }

        private async Task<Result> PublishAchievementPicturesEventAsync(
            CreateAchievementCommand command,
            AchievementEntity achievement,
            string pictureName,
            string sourceKey,
            CancellationToken cancellationToken)
        {
            var @event = new GenerateAchievementsPicturesEvent(
                achievement.Id,
                sourceKey,
                BuildBucketKey(BuildAchievementPictureFolderPath(command.GameId, achievement.Id, SmallPicturesFolderName), pictureName),
                BuildBucketKey(BuildAchievementPictureFolderPath(command.GameId, achievement.Id, MediumPicturesFolderName), pictureName),
                BuildBucketKey(BuildAchievementPictureFolderPath(command.GameId, achievement.Id, LargePicturesFolderName), pictureName),
                new PictureResizeSize(64, 64),
                new PictureResizeSize(128, 128),
                new PictureResizeSize(256, 256));

            try
            {
                await bus.PublishAsync(@event, cancellationToken);
                return Result.Success();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error publishing achievement picture generation event for achievement {AchievementId}", achievement.Id);
                return Result.Error();
            }
        }

        private async Task RemoveUploadedPictureAsync(string sourceKey, CancellationToken cancellationToken)
        {
            try
            {
                await s3Service.RemoveFileAsync(sourceKey, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error removing achievement picture {PictureKey}", sourceKey);
            }
        }

        private async Task RemovePersistedAchievementAsync(AchievementEntity achievement, CancellationToken cancellationToken)
        {
            try
            {
                database.Achievements.Remove(achievement);
                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error rolling back achievement {AchievementId}", achievement.Id);
            }
        }

        private static string NormalizeName(string name)
        {
            return name.Trim().ToUpperInvariant();
        }

        private static string BuildAchievementPictureFolderPath(Guid gameId, Guid achievementId, string folderName)
        {
            return $"games/{gameId}/{AchievementsFolderName}/{achievementId}/{folderName}";
        }

        private static string BuildBucketKey(string route, string fileName)
        {
            return $"{route}/{fileName}";
        }
    }
}