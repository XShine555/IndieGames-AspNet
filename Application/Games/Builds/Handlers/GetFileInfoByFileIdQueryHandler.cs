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
    public class GetFileInfoByFileIdQueryHandler(IDatabase database,
        ILogger<GetFileInfoByFileIdQueryHandler> logger,
        IGameBuildMapper gameBuildMapper)
        : IQueryHandler<GetFileInfoByFileIdQuery, Result<ApplicationFileInfo>>
    {
        public async ValueTask<Result<ApplicationFileInfo>> Handle(GetFileInfoByFileIdQuery query, CancellationToken cancellationToken)
        {
            var file = await database.GameBuildFiles.AsNoTracking()
                .Include(x => x.GameBuild)
                .ThenInclude(x => x.Game)
                .SingleOrDefaultAsync(x => x.Id == query.FileId, cancellationToken);

            if (file is null)
            {
                logger.LogWarning("File with id {FileId} not found", query.FileId);
                return Result.NotFound();
            }

            var ownsGame = await database.Users.AsNoTracking()
                .Where(x => x.IdentityId == query.UserId)
                .SelectMany(x => x.OwnedGames)
                .AnyAsync(x => x.GameId == file.GameBuild.GameId, cancellationToken);

            if (!ownsGame)
            {
                logger.LogWarning("User with id {UserId} does not own game with id {GameId}", query.UserId, file.GameBuild.GameId);
                return Result.Forbidden();
            }

            logger.LogInformation("File with id {FileId} found for user with id {UserId}", query.FileId, query.UserId);
            return Result.Success(gameBuildMapper.ToApplicationFileInfo(file));
        }
    }
}