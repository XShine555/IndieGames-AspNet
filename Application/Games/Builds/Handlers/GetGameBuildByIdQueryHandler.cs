using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Builds.Queries;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class GetGameBuildByIdQueryHandler(
        IDatabase database,
        IGameBuildMapper gameBuildMapper,
        ILogger<GetGameBuildByIdQueryHandler> logger)
        : IQueryHandler<GetGameBuildByIdQuery, Result<ApplicationGameBuild>>
    {
        public async ValueTask<Result<ApplicationGameBuild>> Handle(GetGameBuildByIdQuery query, CancellationToken cancellationToken)
        {
            var buildProjection = await database.GameBuilds
                .AsNoTracking()
                .Where(build => build.Id == query.BuildId)
                .Select(build => new
                {
                    Build = build,
                    build.GameId,
                    OwnerId = build.Game.OwnerId,
                    build.Game.ReleaseGameBuildId,
                })
                .SingleOrDefaultAsync(cancellationToken);

            if (buildProjection is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found", query.BuildId);
                return Result.NotFound();
            }

            var hasInLibrary = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == buildProjection.GameId, cancellationToken);

            var isOwner = buildProjection.OwnerId == query.UserId;
            if (!hasInLibrary && !isOwner)
            {
                logger.LogWarning("User {UserId} is not authorized to get game build {BuildId}", query.UserId, query.BuildId);
                return Result.Unauthorized();
            }

            var build = gameBuildMapper.ToApplicationGameBuild(
                buildProjection.Build,
                buildProjection.ReleaseGameBuildId == buildProjection.Build.Id);

            return Result.Success(build);
        }
    }
}
