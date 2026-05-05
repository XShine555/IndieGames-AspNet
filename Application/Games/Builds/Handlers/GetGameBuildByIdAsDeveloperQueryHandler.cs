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
    public class GetGameBuildByIdAsDeveloperQueryHandler(IDatabase database, ILogger<GetGameBuildByIdAsDeveloperQueryHandler> logger, IGameBuildMapper gameBuildMapper)
        : IQueryHandler<GetGameBuildByIdAsDeveloperQuery, Result<ApplicationGameBuild>>
    {
        public async ValueTask<Result<ApplicationGameBuild>> Handle(GetGameBuildByIdAsDeveloperQuery query, CancellationToken cancellationToken)
        {
            var build = await database.GameBuilds
                .AsNoTracking()
                .Where(b => b.Id == query.BuildId)
                .Include(b => b.Game)
                .SingleOrDefaultAsync(cancellationToken);

            if (build is null)
            {
                logger.LogWarning("Game build with ID {BuildId} not found for user {UserId}", query.BuildId, query.UserId);
                return Result.NotFound("Build not found");
            }

            if (build.Game.OwnerId != query.UserId)
            {
                logger.LogWarning("User {UserId} attempted to access game build with ID {BuildId} which they do not own", query.UserId, query.BuildId);
                return Result.Unauthorized();
            }

            return Result.Success(
                gameBuildMapper.ToApplicationGameBuild(build, build.Id == build.Game.ReleaseGameBuildId)
            );
        }
    }
}
