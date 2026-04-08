using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Queries;
using Application.Games.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Games.Handlers
{
    public class GetGamesQueryHandler(IDatabase database)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
        {
            var normalizedTitle = query.Title.Trim().ToLower();
            var totalCount = await database.Games.CountAsync(cancellationToken);
            var games = await database.Games.AsNoTracking()
                .Where(g => g.NormalizedTitle.Contains(normalizedTitle)
                    || g.Genres.Any(gg => query.Genres.Contains(gg.Id)))
                .Select(g => ApplicationGame.FromEntity(g))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);
            return PaginatedApplicationResponse<ApplicationGame>.FromPagedList(games);
        }
    }
}