using Application.Abstractions.Persistence;
using Application.Configuration;
using Application.Games.Builds.Queries;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Builds.Handlers
{
    public class GetFileInfoByFileIdQueryHandler(
        IDatabase database,
        GameConfiguration gameConfiguration,
        ILogger<GetFileInfoByFileIdQueryHandler> logger)
        : IQueryHandler<GetFileInfoByFileIdQuery, Result<ApplicationFileInfo>>
    {
        public async ValueTask<Result<ApplicationFileInfo>> Handle(GetFileInfoByFileIdQuery query, CancellationToken cancellationToken)
        {
            var file = await database.GameBuildFiles
                .AsNoTracking()
                .Include(f => f.GameBuild)
                    .ThenInclude(b => b.Game)
                .SingleOrDefaultAsync(f => f.Id == query.FileId, cancellationToken);

            if (file is null)
            {
                logger.LogWarning("Game build file {FileId} not found", query.FileId);
                return Result.NotFound();
            }

            if (!file.GameBuild.Game.IsPublished || file.GameBuild.Status != GameBuildStatus.Completed)
            {
                logger.LogWarning("Game build file {FileId} is not available", query.FileId);
                return Result.NotFound();
            }

            var ownsGame = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(o => o.UserId == query.UserId && o.GameId == file.GameBuild.GameId, cancellationToken);

            if (!ownsGame)
            {
                logger.LogWarning("User {UserId} is not authorized to access file {FileId}", query.UserId, query.FileId);
                return Result.Unauthorized();
            }

            var fullPath = gameConfiguration.Routes.BuildGameBuildFilePath(
                file.GameBuild.GameId, file.GameBuildId, file.FileRelativePath);

            return Result.Success(new ApplicationFileInfo(
                file.Id,
                file.GameBuildId,
                fullPath,
                file.FileSize,
                file.Hash,
                file.HashAlgorithm));
        }
    }
}
