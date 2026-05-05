using Application.Abstractions.Persistence;
using Application.Games.Builds.Queries;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Games.Builds.Handlers
{
    public class GetFileListByGameBuildIdQueryHandler(IDatabase database)
        : IQueryHandler<GetFileListByGameBuildIdQuery, Result<IReadOnlyList<string> >>
    {
        public async ValueTask<Result<IReadOnlyList<string>> > Handle(GetFileListByGameBuildIdQuery query, CancellationToken cancellationToken)
        {
            var build = await database.GameBuilds
                .AsNoTracking()
                .Include(x => x.Game)
                .Where(x => x.Id == query.BuildId)
                .SingleOrDefaultAsync(cancellationToken);

            if (build is null)
                return Result.NotFound("Build not found");

            if (build.Game.OwnerId != query.UserId)
                return Result.Unauthorized();

            var files = await database.GameBuildFiles
                .AsNoTracking()
                .Where(x => x.GameBuildId == query.BuildId)
                .Select(x => x.FileRelativePath + x.FileName)
                .ToListAsync(cancellationToken);

            return Result<IReadOnlyList<string>>.Success(files);
        }
    }
}