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
    public class GetGameBuildByIdAsUserQueryHandler(
        IDatabase database,
        IGameBuildMapper gameBuildMapper,
        ILogger<GetGameBuildByIdAsUserQueryHandler> logger)
        : IQueryHandler<GetGameBuildByIdQuery, Result<ApplicationGameBuild>>
    {
        public async ValueTask<Result<ApplicationGameBuild>> Handle(GetGameBuildByIdQuery query, CancellationToken cancellationToken)
        {
            var build = await database.GameBuilds
                .AsNoTracking()
                .Where(build => build.Id == query.BuildId)
                .Include(build => build.Game)
                .SingleOrDefaultAsync(cancellationToken);

            if (build is null)
            {
                logger.LogWarning("Game build with id {BuildId} not found", query.BuildId);
                return Result.NotFound();
            }

            if (!build.Game.IsPublished || build.Status != GameBuildStatus.Completed)
            {
                logger.LogWarning("Game build with id {BuildId} not found for user mode", query.BuildId);
                return Result.NotFound();
            }

            var hasInLibrary = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == build.GameId, cancellationToken);

            if (!hasInLibrary)
            {
                logger.LogWarning("User {UserId} is not authorized to get game build {BuildId}", query.UserId, query.BuildId);
                return Result.Unauthorized();
            }

            return Result.Success(
                gameBuildMapper.ToApplicationGameBuild(build, build.Game.ReleaseGameBuildId == build.Id)
            );
        }
    }
}
