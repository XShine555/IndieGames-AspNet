using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Genres.Handlers
{
    public class GetGenresQueryHandler(IDatabase database)
        : IQueryHandler<GetGenresQuery, PaginatedApplicationResponse<ApplicationGenre>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGenre>> Handle(GetGenresQuery query, CancellationToken cancellationToken)
        {
            var normalizedName = query.Name.Trim().ToLowerInvariant();
            var totalCount = await database.Genres.CountAsync(cancellationToken);
            var genres = await database.Genres
                .AsNoTracking()
                .Where(x => x.Name.Contains(normalizedName))
                .Select(x => ApplicationGenre.FromEntity(x))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);
            return PaginatedApplicationResponse<ApplicationGenre>.FromPagedList(genres);
        }
    }
}