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

namespace Application.Games.Builds.Handlers
{
    public class GetGameBuildsAsUserQueryHandler(
        IDatabase database,
        IGameBuildMapper gameBuildMapper,
        ILogger<GetGameBuildsAsUserQueryHandler> logger)
        : IQueryHandler<GetGameBuildsAsUserQuery, Result<PaginatedApplicationResponse<ApplicationGameBuild>> >
    {
        public async ValueTask<Result<PaginatedApplicationResponse<ApplicationGameBuild> >> Handle(GetGameBuildsAsUserQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .Where(g => g.Id == query.GameId)
                .SingleOrDefaultAsync(cancellationToken);

            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found when listing builds", query.GameId);
                return Result.NotFound();
            }

            var hasInLibrary = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == query.GameId, cancellationToken);

            if (!hasInLibrary)
            {
                logger.LogWarning("User {UserId} is not authorized to list builds for game {GameId}", query.UserId, query.GameId);
                return Result.Unauthorized();
            }

            var buildsQuery = database.GameBuilds
                .AsNoTracking()
                .Where(build => build.GameId == query.GameId & build.Status == GameBuildStatus.Completed);

            if (!string.IsNullOrEmpty(query.Title))
                buildsQuery = buildsQuery.Where(build => build.VersionName.Contains(query.Title));

            var totalCount = await buildsQuery.CountAsync(cancellationToken);
            var builds = await buildsQuery
                .OrderByDescending(build => build.CreatedAt)
                .Select(build => gameBuildMapper.ToApplicationGameBuild(build, game.ReleaseGameBuildId == build.Id))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return Result.Success(
                PaginatedApplicationResponse<ApplicationGameBuild>.FromPagedList(builds)
            );
        }
    }
}
