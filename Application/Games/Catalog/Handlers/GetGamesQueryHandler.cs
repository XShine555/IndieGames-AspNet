using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Queries;
using Application.Games.Catalog.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Application.Games.Catalog.Handlers
{
    public class GetGamesQueryHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGameListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGameListItem>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
        {
            var baseQuery = database.Games
                .AsNoTracking()
                .AsQueryable();

            switch (query.Mode)
            {
                case GameCatalogQueryMode.User:
                    baseQuery = baseQuery.Where(g => g.IsPublished);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var normalizedTitle = query.Title.Trim().ToUpperInvariant();
                baseQuery = baseQuery.Where(g => g.NormalizedTitle.Contains(normalizedTitle));
            }

            if (query.Genres?.Count > 0)
            {
                baseQuery = baseQuery.Where(g => g.Genres.Any(gg => query.Genres.Contains(gg.Id)));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var games = await baseQuery
                .OrderByDescending(g => g.CreatedAt)
                .Select(gameCatalogMapper.ToApplicationGameListItemFunction)
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return PaginatedApplicationResponse<ApplicationGameListItem>.FromPagedList(games);
        }
    }
}
