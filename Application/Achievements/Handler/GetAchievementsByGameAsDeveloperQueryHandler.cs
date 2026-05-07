using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Achievements.Handler
{
    public class GetAchievementsByGameAsDeveloperQueryHandler(
        IDatabase database,
        ILogger<GetAchievementsByGameAsDeveloperQueryHandler> logger)
        : IQueryHandler<GetAchievementsByGameAsDeveloperQuery, Result<IReadOnlyList<ApplicationAchievement>>>
    {
        public async ValueTask<Result<IReadOnlyList<ApplicationAchievement> >> Handle(GetAchievementsByGameAsDeveloperQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == query.GameId, cancellationToken);

            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found while fetching achievements as developer", query.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != query.UserId)
            {
                logger.LogWarning("User {UserId} attempted to fetch developer achievements for game {GameId} without owning it", query.UserId, query.GameId);
                return Result.Unauthorized();
            }

            var achievements = await database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId)
                .OrderBy(a => a.Name)
                .Select(a => new ApplicationAchievement(
                    a.Id,
                    a.GameId,
                    a.Name,
                    a.Description,
                    a.AchievementPicture != null ? a.AchievementPicture.SmallRelativePath : null,
                    a.AchievementPicture != null ? a.AchievementPicture.MediumRelativePath : null,
                    a.AchievementPicture != null ? a.AchievementPicture.LargeRelativePath : null,
                    a.AchievementPicture.ProcessingStatus,
                    a.CreatedAt,
                    a.UpdatedAt))
                .ToListAsync(cancellationToken);

            return Result.Success<IReadOnlyList<ApplicationAchievement>>(achievements);
        }
    }
}
