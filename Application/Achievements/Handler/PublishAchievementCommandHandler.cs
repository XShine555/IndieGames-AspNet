using Application.Abstractions.Persistence;
using Application.Achievements.Commands;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class PublishAchievementCommandHandler(
        IDatabase database,
        ILogger<PublishAchievementCommandHandler> logger)
        : ICommandHandler<PublishAchievementCommand, Result>
    {
        public async ValueTask<Result> Handle(PublishAchievementCommand command, CancellationToken cancellationToken)
        {
            var achievement = await database.Achievements
                .Include(a => a.Game)
                .Include(a => a.AchievementPicture)
                .SingleOrDefaultAsync(a => a.Id == command.AchievementId, cancellationToken);

            if (achievement is null)
            {
                logger.LogWarning("Achievement with id {AchievementId} not found for publish", command.AchievementId);
                return Result.NotFound();
            }

            if (achievement.Game.OwnerId != command.UserId)
            {
                logger.LogWarning(
                    "User {UserId} is not the owner of game {GameId} and cannot publish achievement {AchievementId}",
                    command.UserId,
                    achievement.GameId,
                    achievement.Id);
                return Result.Forbidden();
            }

            if (achievement.IsPublished)
            {
                logger.LogWarning("Achievement with id {AchievementId} is already published", achievement.Id);
                return Result.Conflict("Achievement is already published");
            }

            if (achievement.AchievementPicture is null)
            {
                logger.LogWarning("Achievement {AchievementId} has no picture record and cannot be published", achievement.Id);
                return Result.Invalid(new List<ValidationError> { new("Achievement must have a picture") });
            }

            if (achievement.AchievementPicture.ProcessingStatus != AchievementPictureProcessingStatus.Completed)
            {
                logger.LogWarning(
                    "Achievement {AchievementId} cannot be published because picture status is {Status}",
                    achievement.Id,
                    achievement.AchievementPicture.ProcessingStatus);

                return Result.Invalid(new List<ValidationError> { new("Achievement picture processing must be completed") });
            }

            achievement.IsPublished = true;
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Achievement {AchievementId} published by user {UserId}", achievement.Id, command.UserId);
            return Result.Success();
        }
    }
}
