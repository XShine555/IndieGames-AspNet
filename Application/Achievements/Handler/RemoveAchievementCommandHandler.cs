using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Achievements.Commands;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class RemoveAchievementCommandHandler(
        IDatabase database,
        IS3Service s3Service,
        ILogger<RemoveAchievementCommandHandler> logger)
        : ICommandHandler<RemoveAchievementCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveAchievementCommand command, CancellationToken cancellationToken)
        {
            var achievement = await database.Achievements
                .AsNoTracking()
                .Include(a => a.Game)
                .Include(a => a.AchievementPicture)
                .SingleOrDefaultAsync(a => a.Id == command.AchievementId, cancellationToken);

            if (achievement is null)
            {
                logger.LogWarning("Achievement with id {AchievementId} not found for deletion", command.AchievementId);
                return Result.NotFound("Achievement not found");
            }

            if (achievement.Game.OwnerId != command.UserId)
            {
                logger.LogWarning(
                    "User {UserId} is not authorized to delete achievement {AchievementId}",
                    command.UserId,
                    command.AchievementId);
                return Result.Unauthorized();
            }

            if (achievement.AchievementPicture is null)
            {
                logger.LogWarning("Achievement {AchievementId} has no picture record and cannot be deleted", command.AchievementId);
                return Result.Conflict("Achievement picture is required before deleting the achievement");
            }

            if (achievement.AchievementPicture.ProcessingStatus != AchievementPictureProcessingStatus.Completed
                && achievement.AchievementPicture.ProcessingStatus != AchievementPictureProcessingStatus.Failed)
            {
                logger.LogWarning(
                    "Achievement {AchievementId} cannot be deleted because picture is in status {Status}",
                    command.AchievementId,
                    achievement.AchievementPicture.ProcessingStatus);
                return Result.Conflict("Cannot delete achievement while its picture is being processed");
            }

            var keys = GetKeysToDelete(achievement.AchievementPicture);
            foreach (var key in keys)
            {
                try
                {
                    await s3Service.RemoveFileAsync(key, cancellationToken);
                }
                catch (Exception exception)
                {
                    logger.LogError(exception,
                        "Error removing achievement picture file {Key} for achievement {AchievementId}",
                        key,
                        achievement.Id);
                    return Result.Error("An error occurred while removing achievement pictures");
                }
            }

            try
            {
                var persistedAchievement = await database.Achievements
                    .Include(a => a.Game)
                    .SingleOrDefaultAsync(a => a.Id == command.AchievementId, cancellationToken);

                if (persistedAchievement is null)
                {
                    logger.LogWarning("Achievement with id {AchievementId} not found for deletion", command.AchievementId);
                    return Result.NotFound("Achievement not found");
                }

                database.Achievements.Remove(persistedAchievement);
                await database.SaveChangesAsync(cancellationToken);

                logger.LogInformation(
                    "Achievement with id {AchievementId} successfully deleted by user {UserId}",
                    command.AchievementId,
                    command.UserId);

                return Result.NoContent();
            }
            catch (Exception exception)
            {
                logger.LogError(exception,
                    "Error deleting achievement {AchievementId} by user {UserId}",
                    command.AchievementId,
                    command.UserId);
                return Result.Error("An error occurred while deleting the achievement");
            }
        }

        private static IReadOnlyList<string> GetKeysToDelete(Domain.Entities.AchievementPicture picture)
        {
            var keys = new List<string>(capacity: 4)
            {
                $"{picture.OriginalRelativePath}/{picture.OriginalName}"
            };

            if (!string.IsNullOrWhiteSpace(picture.SmallRelativePath) && !string.IsNullOrWhiteSpace(picture.SmallName))
                keys.Add($"{picture.SmallRelativePath}/{picture.SmallName}");

            if (!string.IsNullOrWhiteSpace(picture.MediumRelativePath) && !string.IsNullOrWhiteSpace(picture.MediumName))
                keys.Add($"{picture.MediumRelativePath}/{picture.MediumName}");

            if (!string.IsNullOrWhiteSpace(picture.LargeRelativePath) && !string.IsNullOrWhiteSpace(picture.LargeName))
                keys.Add($"{picture.LargeRelativePath}/{picture.LargeName}");

            return keys;
        }
    }
}
