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
            var games = database.Games
                .AsNoTracking()
                .AsQueryable()
                .Where(g => g.OwnerId == query.UserId);

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var normalizedTitle = query.Title.Trim().ToUpperInvariant();
                games = games.Where(g => g.NormalizedTitle.Contains(normalizedTitle));
            }

            var totalCount = await games.CountAsync(cancellationToken);

            var gameList = await games.ToListAsync(cancellationToken);
            var pagedList = gameList.Select(mapper.ToApplicationGame)
                .ToPagedList(query.PageNumber, query.PageSize, totalCount);

            return PaginatedApplicationResponse<ApplicationGame>.FromPagedList(pagedList);
        }
    }
}