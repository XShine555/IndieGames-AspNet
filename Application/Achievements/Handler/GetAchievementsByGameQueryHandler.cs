using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Achievements.Queries;
using Application.Achievements.Responses;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Application.Achievements.Handler
{
    public class GetAchievementsByGameQueryHandler(IDatabase database, IAchievementMapper achievementMapper)
        : IQueryHandler<GetAchievementsByGameQuery, PaginatedApplicationResponse<ApplicationAchievement>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationAchievement>> Handle(
            GetAchievementsByGameQuery query,
            CancellationToken cancellationToken)
        {
            var baseQuery = database.Achievements
                .AsNoTracking()
                .Where(a => a.GameId == query.GameId)
                .Where(a => a.IsPublished)
                .OrderBy(a => a.Name)
                .Select(a => new ApplicationAchievement(
                    a.Id,
                    a.GameId,
                    a.Name,
                    a.Description,
                    a.AchievementPicture.SmallRelativePath + "/" + a.AchievementPicture.SmallName,
                    a.AchievementPicture.MediumRelativePath + "/" + a.AchievementPicture.MediumName,
                    a.AchievementPicture.LargeRelativePath + "/" + a.AchievementPicture.LargeName,
                    a.IsPublished,
                    a.AchievementPicture.ProcessingStatus,
                    a.CreatedAt,
                    a.UpdatedAt))
                .AsQueryable();

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var achievements = await baseQuery.ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return PaginatedApplicationResponse<ApplicationAchievement>.FromPagedList(achievements);
        }
    }
}
