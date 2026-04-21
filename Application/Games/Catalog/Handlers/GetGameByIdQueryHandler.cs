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
            var gameQuery = database.Games
                .AsNoTracking()
                .AsSplitQuery()
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .Include(g => g.ReleaseGameBuild)
                .Where(g => g.Id == query.Id);

            switch (query.Mode)
            {
                case GameCatalogQueryMode.User:
                    gameQuery = gameQuery.Where(g =>
                        g.IsPublished);
                    break;
            }

            var game = await gameQuery.SingleOrDefaultAsync(cancellationToken);

            if (game is null)
                return Result.NotFound();

            return Result.Success(gameCatalogMapper.ToApplicationGame(game));
        }
    }
}
