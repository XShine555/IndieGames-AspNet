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
    public class GetGamesByOwnerIdQueryHandler(IDatabase database, IGameCatalogMapper gameCatalogMapper)
        : IQueryHandler<GetGamesByOwnerIdQuery, PaginatedApplicationResponse<ApplicationGameListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGameListItem>> Handle(GetGamesByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            var baseQuery = database.Games.AsNoTracking()
                .Where(g => g.OwnerId == query.UserId);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var games = await baseQuery
                .OrderByDescending(g => g.CreatedAt)
                .Select(gameCatalogMapper.ToApplicationGameListItemFunction)
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            return PaginatedApplicationResponse<ApplicationGameListItem>.FromPagedList(games);
        }
    }
}