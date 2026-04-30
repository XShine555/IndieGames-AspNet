using Application.Abstractions.Persistence;
using Application.Achievements.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class UnlockAchievementCommandHandler(
        IDatabase database,
        ILogger<UnlockAchievementCommandHandler> logger)
        : ICommandHandler<UnlockAchievementCommand, Result>
    {
        public async ValueTask<Result> Handle(UnlockAchievementCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.IdentityId == command.UserId, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User {UserId} not found while unlocking achievement {AchievementId}", command.UserId, command.AchievementId);
                return Result.NotFound("User not found.");
            }

            var achievement = await database.Achievements
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == command.AchievementId, cancellationToken);

            if (achievement is null)
            {
                logger.LogWarning("Achievement {AchievementId} not found", command.AchievementId);
                return Result.NotFound("Achievement not found.");
            }

            var ownsGame = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(l => l.UserId == user.IdentityId && l.GameId == achievement.GameId, cancellationToken);

            if (!ownsGame)
            {
                logger.LogWarning("User {UserId} does not own game {GameId} required for achievement {AchievementId}", command.UserId, achievement.GameId, command.AchievementId);
                return Result.Forbidden("User does not own the game.");
            }

            var alreadyUnlocked = await database.UserAchievements
                .AsNoTracking()
                .AnyAsync(ua => ua.UserId == user.IdentityId && ua.AchievementId == command.AchievementId, cancellationToken);

            if (alreadyUnlocked)
                return Result.Conflict("Achievement already unlocked.");

            var userAchievement = new UserAchievement
            {
                UserId = user.IdentityId,
                AchievementId = command.AchievementId,
                UnlockedAt = DateTime.UtcNow
            };

            await database.UserAchievements.AddAsync(userAchievement, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {UserId} unlocked achievement {AchievementId}", user.IdentityId, command.AchievementId);
            return Result.Success();
        }
    }
}
