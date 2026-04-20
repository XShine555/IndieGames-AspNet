using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class GetGameBuildsQueryHandler(
        IDatabase database,
        IGameMapper gameMapper,
        ILogger<GetGameBuildsQueryHandler> logger)
        : IQueryHandler<GetGameBuildsQuery, Result<IReadOnlyCollection<ApplicationGameBuild>>>
    {
        public async ValueTask<Result<IReadOnlyCollection<ApplicationGameBuild>>> Handle(GetGameBuildsQuery query, CancellationToken cancellationToken)
        {
            var gameProjection = await database.Games
                .AsNoTracking()
                .Where(g => g.Id == query.GameId)
                .Select(g => new { g.Id, g.OwnerId, g.ReleaseGameBuildId } )
                .SingleOrDefaultAsync(cancellationToken);
            if (gameProjection is null)
            {
                logger.LogWarning("Game with id {GameId} not found when listing builds", query.GameId);
                return Result.NotFound();
            }

            var hasInLibrary = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(owned => owned.UserId == query.UserId && owned.GameId == query.GameId, cancellationToken);
            var isOwner = gameProjection.OwnerId == query.UserId;
            if (!hasInLibrary && !isOwner)
            {
                logger.LogWarning("User {UserId} is not authorized to list builds for game {GameId}", query.UserId, query.GameId);
                return Result.Unauthorized();
            }

            var builds = await database.GameBuilds
                .AsNoTracking()
                .Where(build => build.GameId == query.GameId)
                .OrderByDescending(build => build.CreatedAt)
                .ToArrayAsync(cancellationToken);

            IReadOnlyCollection<ApplicationGameBuild> mappedBuilds = builds
                .Select(build => gameMapper.ToApplicationGameBuild(build, gameProjection.ReleaseGameBuildId == build.Id))
                .ToArray();

            return Result.Success(mappedBuilds);
        }
    }
}
