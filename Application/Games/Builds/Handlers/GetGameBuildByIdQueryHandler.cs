using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Builds.Queries;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Entities;
using Domain.Games.Enums;
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
        private record BuildProjection(
            GameBuild Build,
            Guid GameId,
            Guid OwnerId,
            Guid? ReleaseGameBuildId,
            bool IsGamePublished);

        public async ValueTask<Result<ApplicationGameBuild>> Handle(GetGameBuildByIdQuery query, CancellationToken cancellationToken)
        {
            var buildProjection = await database.GameBuilds
                .AsNoTracking()
                .Where(build => build.Id == query.BuildId)
                .Select(build => new BuildProjection(
                    build,
                    build.GameId,
                    build.Game.OwnerId,
                    build.Game.ReleaseGameBuildId,
                    build.Game.IsPublished))
                .SingleOrDefaultAsync(cancellationToken);

            if (buildProjection is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found", query.BuildId);
                return Result.NotFound();
            }

            switch (query.Mode)
            {
                case GameBuildQueryMode.Developer when buildProjection.OwnerId != query.UserId:
                    logger.LogWarning("User {UserId} is not authorized to get developer game build {BuildId}", query.UserId, query.BuildId);
                    return Result.Unauthorized();

                case GameBuildQueryMode.User:
                    if (!buildProjection.IsGamePublished || buildProjection.Build.Status != GameBuildStatus.Completed)
                    {
                        logger.LogWarning("Game build with id {BuildId} not found for user mode", query.BuildId);
                        return Result.NotFound();
                    }

                    var hasInLibrary = await database.UserLibrary
                        .AsNoTracking()
                        .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == buildProjection.GameId, cancellationToken);

                    if (!hasInLibrary)
                    {
                        logger.LogWarning("User {UserId} is not authorized to get game build {BuildId}", query.UserId, query.BuildId);
                        return Result.Unauthorized();
                    }
                    break;

                case GameBuildQueryMode.Developer:
                    break;
            }

            var build = gameBuildMapper.ToApplicationGameBuild(
                buildProjection.Build,
                buildProjection.ReleaseGameBuildId == buildProjection.Build.Id);

            return Result.Success(build);
        }
    }
}
