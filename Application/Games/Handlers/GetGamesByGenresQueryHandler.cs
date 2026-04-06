using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Queries;
using Application.Games.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Games.Handlers
{
    public class GetGamesByGenresQueryHandler(IDatabase database)
        : IQueryHandler<GetGamesByGenresQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetGamesByGenresQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var totalGames = await database.Games.CountAsync(cancellationToken);
            var pagedGames = await database.Games
                .AsNoTracking()
                .OrderBy(game => game.Id)
                .Select(game => ApplicationGame.FromEntity(game))
                .ToPagedListAsync(pageNumber, pageSize, totalGames, cancellationToken);

            return PaginatedApplicationResponse<ApplicationGame>.FromPagedList(pagedGames);
        }
    }
}