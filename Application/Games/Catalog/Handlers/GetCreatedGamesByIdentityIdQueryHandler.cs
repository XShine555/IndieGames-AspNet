using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Queries;
using Application.Games.Catalog.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Application.Games.Catalog.Handlers
{
    public class GetCreatedGamesByIdentityIdQueryHandler(IDatabase database, IGameCatalogMapper mapper)
        : IQueryHandler<GetCreatedGamesByIdentityIdQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetCreatedGamesByIdentityIdQuery query, CancellationToken cancellationToken)
        {
            var totalCount = await database.Games
                .Where(g => g.OwnerId == query.UserId)
                .CountAsync(cancellationToken);

            var games = await database.Games
                .AsNoTracking()
                .Where(g => g.OwnerId == query.UserId)
                .ToListAsync(cancellationToken);

            var pagedList = games.Select(mapper.ToApplicationGame)
                .ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationGame>.FromPagedList(pagedList);
        }
    }
}