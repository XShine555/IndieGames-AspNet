using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Queries;
using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Games.Catalog.Handlers
{
    public class GetGameByIdQueryHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper)
        : IQueryHandler<GetGameByIdQuery, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .AsSplitQuery()
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .Include(g => g.ReleaseGameBuild)
                .SingleOrDefaultAsync(q => q.Id == query.Id, cancellationToken);

            if (game is null)
                return Result.NotFound();

            return Result.Success(gameCatalogMapper.ToApplicationGame(game));
        }
    }
}
