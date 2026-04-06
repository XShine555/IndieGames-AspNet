using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Queries;
using Application.Games.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Games.Handlers
{
    public class GetGamesByTitleQueryHandler(IDatabase database)
        : IQueryHandler<GetGamesByTitleQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetGamesByTitleQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var totalGames = await database.Games.CountAsync(cancellationToken);
            var normalizedTitle = query.Title.Trim().ToUpperInvariant();
            var games = await database.Games.AsNoTracking()
                .Where(game => game.NormalizedTitle == normalizedTitle)
                .Select(game => ApplicationGame.FromEntity(game))
                .ToPagedListAsync(pageNumber, pageSize, totalGames, cancellationToken);

            return PaginatedApplicationResponse<ApplicationGame>.FromPagedList(games);
        }
    }
}