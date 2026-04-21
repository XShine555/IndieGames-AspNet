using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Builds.Queries;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class GetGameBuildsQueryHandler(
        IDatabase database,
        IGameBuildMapper gameBuildMapper,
        ILogger<GetGameBuildsQueryHandler> logger)
        : IQueryHandler<GetGameBuildsQuery, Result<IReadOnlyCollection<ApplicationGameBuild>>>
    {
        private record GameProjection(
            Guid GameId,
            Guid OwnerId,
            Guid? ReleaseGameBuildId);

        public async ValueTask<Result<IReadOnlyCollection<ApplicationGameBuild>>> Handle(GetGameBuildsQuery query, CancellationToken cancellationToken)
        {
            var gameProjection = await database.Games
                .AsNoTracking()
                .Where(g => g.Id == query.GameId)
                .Select(g => new GameProjection(g.Id, g.OwnerId, g.ReleaseGameBuildId))
                .SingleOrDefaultAsync(cancellationToken);

            if (gameProjection is null)
            {
                logger.LogWarning("Game with id {GameId} not found when listing builds", query.GameId);
                return Result.NotFound();
            }

            switch (query.Mode)
            {
                case GameBuildQueryMode.Developer when gameProjection.OwnerId != query.UserId:
                    logger.LogWarning("User {UserId} is not authorized to list developer builds for game {GameId}", query.UserId, query.GameId);
                    return Result.Unauthorized();

                case GameBuildQueryMode.User:
                    var hasInLibrary = await database.UserLibrary
                        .AsNoTracking()
                        .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == query.GameId, cancellationToken);

                    if (!hasInLibrary)
                    {
                        logger.LogWarning("User {UserId} is not authorized to list builds for game {GameId}", query.UserId, query.GameId);
                        return Result.Unauthorized();
                    }
                    break;

                case GameBuildQueryMode.Developer:
                    break;
            }

            var buildsQuery = database.GameBuilds
                .AsNoTracking()
                .Where(build => build.GameId == query.GameId);

            if (query.Mode == GameBuildQueryMode.User)
            {
                buildsQuery = buildsQuery.Where(build => build.Status == GameBuildStatus.Completed);
            }

            var builds = await buildsQuery
                .OrderByDescending(build => build.CreatedAt)
                .ToArrayAsync(cancellationToken);

            IReadOnlyCollection<ApplicationGameBuild> mappedBuilds = builds
                .Select(build => gameBuildMapper.ToApplicationGameBuild(build, gameProjection.ReleaseGameBuildId == build.Id))
                .ToArray();

            return Result.Success(mappedBuilds);
        }
    }
}
