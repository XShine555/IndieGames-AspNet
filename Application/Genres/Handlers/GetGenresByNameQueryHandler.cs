using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Genres.Handlers
{
    public class GetGenresByNameQueryHandler(IDatabase database)
        : IQueryHandler<GetGenresByNameQuery, PaginatedApplicationResponse<ApplicationGenre>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGenre>> Handle(GetGenresByNameQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

            var totalGenres = await database.Genres.CountAsync(cancellationToken);
            var genres = await database.Genres
                .AsNoTracking()
                .Where(g => g.Name.Contains(query.Name))
                .Select(g => ApplicationGenre.FromEntity(g))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalGenres, cancellationToken);

            return PaginatedApplicationResponse<ApplicationGenre>.FromPagedList(genres);
        }
    }
}