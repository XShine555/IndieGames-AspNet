using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Builds.Queries;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Application.Games.Builds.Handlers
{
    public class GetGameBuildsAsDeveloperQueryHandler(IDatabase database, ILogger<GetGameBuildsAsDeveloperQueryHandler> logger, IGameBuildMapper gameBuildMapper)
        : IQueryHandler<GetGameBuildsAsDeveloperQuery, Result<PaginatedApplicationResponse<ApplicationGameBuildListItem> >>
    {
        public async ValueTask<Result<PaginatedApplicationResponse<ApplicationGameBuildListItem> >> Handle(GetGameBuildsAsDeveloperQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .Where(g => g.Id == query.GameId)
                .Select(g => new { g.Id, g.OwnerId, g.ReleaseGameBuildId })
                .SingleOrDefaultAsync(cancellationToken);

            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found when listing builds", query.GameId);
                return Result.NotFound("Game not found");
            }

            if (game.OwnerId != query.UserId)
            {
                logger.LogWarning("User {UserId} is not authorized to list builds for game {GameId}", query.UserId, query.GameId);
                return Result.Unauthorized();
            }

            var buildsQuery = database.GameBuilds
                .AsNoTracking()
                .Where(build => build.GameId == query.GameId);

            if (!string.IsNullOrEmpty(query.Title))
            {
                buildsQuery = buildsQuery
                    .Where(build => build.VersionName.Contains(query.Title));
            }

            var totalCount = await buildsQuery.CountAsync(cancellationToken);

            var builds = await buildsQuery
                .OrderByDescending(build => build.CreatedAt)
                .Select(build =>
                    gameBuildMapper.ToApplicationGameBuildListItem(
                        build,
                        build.Id == game.ReleaseGameBuildId
                    ))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return Result.Success(
                PaginatedApplicationResponse<ApplicationGameBuildListItem>.FromPagedList(builds)
            );
        }
    }
}
