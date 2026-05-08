using Application.Abstractions.Common;
using Application.Abstractions.Messaging;
using Application.Abstractions.Messaging.Achievements.V1;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Achievements.Commands;
using Application.Achievements.Responses;
using Application.Configuration;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class CreateAchievementCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        IEventBus bus,
        AchievementsConfiguration achievementsConfiguration,
        IAchievementMapper achievementMapper,
        ILogger<CreateAchievementCommandHandler> logger)
        : ICommandHandler<CreateAchievementCommand, Result<ApplicationAchievement>>
    {
        public async ValueTask<Result<ApplicationAchievement>> Handle(CreateAchievementCommand command, CancellationToken cancellationToken)
        {
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

            var achievementsCount = await database.Achievements
                .AsNoTracking()
                .CountAsync(a => a.GameId == command.GameId, cancellationToken);

            if (achievementsCount >= achievementsConfiguration.MaxAchievementsPerGame)
            {
                logger.LogWarning(
                    "Max achievements limit reached ({MaxAchievementsPerGame}) for game {GameId}. User {UserId} attempted to create another achievement.",
                    achievementsConfiguration.MaxAchievementsPerGame,
                    command.GameId,
                    command.UserId);
                return Result.Conflict($"Maximum achievements limit reached ({achievementsConfiguration.MaxAchievementsPerGame}).");
            }

            var achievement = new Achievement
            {
                GameId = command.GameId,
                Name = command.Name.Trim(),
                NormalizedName = normalizedName,
                Description = command.Description.Trim()
            };

            var pictureName = Guid.NewGuid() + command.Picture.FileExtension;
            var sourceKey = achievementsConfiguration.Routes.BuildBucketKey(
                achievementsConfiguration.Routes.GetPicturesFolderPath(command.GameId, achievement.Id),
                pictureName);

            var achievementPicture = new AchievementPicture
            {
                AchievementId = achievement.Id,
                OriginalName = pictureName,
                OriginalRelativePath = achievementsConfiguration.Routes.GetPicturesFolderPath(command.GameId, achievement.Id),
                OriginalContentType = command.Picture.ContentType,
            };

            achievement.AchievementPicture = achievementPicture;

            var uploadResult = await UploadOriginalPictureAsync(command.Picture, sourceKey, cancellationToken);
            if (!uploadResult.IsSuccess)
                return Result.Error("Error uploading achievement picture");

            await database.Achievements.AddAsync(achievement, cancellationToken);
            await database.AchievementPictures.AddAsync(achievementPicture, cancellationToken);
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
            Achievement achievement,
            string pictureName,
            string sourceKey,
            CancellationToken cancellationToken)
        {
            var @event = new GenerateAchievementsPicturesEvent(
                achievement.Id,
                sourceKey,
                achievementsConfiguration.Routes.BuildBucketKey(
                    achievementsConfiguration.Routes.GetSmallPicturesFolderPath(command.GameId, achievement.Id),
                    pictureName),
                achievementsConfiguration.Routes.BuildBucketKey(
                    achievementsConfiguration.Routes.GetMediumPicturesFolderPath(command.GameId, achievement.Id),
                    pictureName),
                achievementsConfiguration.Routes.BuildBucketKey(
                    achievementsConfiguration.Routes.GetLargePicturesFolderPath(command.GameId, achievement.Id),
                    pictureName),
                new PictureResizeSize(achievementsConfiguration.Sizes.Small.Width, achievementsConfiguration.Sizes.Small.Height),
                new PictureResizeSize(achievementsConfiguration.Sizes.Medium.Width, achievementsConfiguration.Sizes.Medium.Height),
                new PictureResizeSize(achievementsConfiguration.Sizes.Large.Width, achievementsConfiguration.Sizes.Large.Height));

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

        private async Task RemovePersistedAchievementAsync(Achievement achievement, CancellationToken cancellationToken)
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
    }
}