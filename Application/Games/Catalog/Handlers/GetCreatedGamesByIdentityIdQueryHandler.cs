using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Queries;
using Application.Games.Catalog.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Application.Games.Catalog.Handlers
{
    public class GetCreatedGamesByIdentityIdQueryHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper)
        : IQueryHandler<GetCreatedGamesByIdentityIdQuery, PaginatedApplicationResponse<ApplicationGameListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGameListItem>> Handle(
            GetCreatedGamesByIdentityIdQuery query,
            CancellationToken cancellationToken)
        {
            var baseQuery = database.Games
                .AsNoTracking()
                .Where(g => g.OwnerId == query.IdentityId);

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var normalizedTitle = query.Title.Trim().ToUpperInvariant();
                baseQuery = baseQuery.Where(g => g.NormalizedTitle.Contains(normalizedTitle));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var games = await baseQuery
                .OrderByDescending(g => g.CreatedAt)
                .Select(gameCatalogMapper.ToApplicationGameListItemFunction)
                .ToListAsync(cancellationToken);

            var pagedList = games.ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationGameListItem>.FromPagedList(pagedList);
        }
    }
}
