using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Achievements.Handler
{
    public class GetAchievementByIdQueryHandler(IDatabase database, IAchievementMapper achievementMapper)
        : IQueryHandler<GetAchievementByIdQuery, Result<ApplicationAchievement>>
    {
        public async ValueTask<Result<ApplicationAchievement>> Handle(
            GetAchievementByIdQuery query,
            CancellationToken cancellationToken)
        {
            var achievement = await database.Achievements
                .AsNoTracking()
                .SingleOrDefaultAsync(a => a.Id == query.AchievementId, cancellationToken);

            if (achievement is null)
                return Result.NotFound();

            return Result.Success(achievementMapper.ToApplicationAchievement(achievement));
        }
    }
}
