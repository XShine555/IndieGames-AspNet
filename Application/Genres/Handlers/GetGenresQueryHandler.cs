using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Genres.Handlers
{
    public class GetGenresQueryHandler(
        IDatabase database,
        IGenreMapper genreMapper)
        : IQueryHandler<GetGenresQuery, PaginatedApplicationResponse<ApplicationGenre>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGenre>> Handle(GetGenresQuery query, CancellationToken cancellationToken)
        {
            var normalizedName = query.Name.Trim().ToUpperInvariant();
            var hasNameFilter = !string.IsNullOrWhiteSpace(normalizedName);

            var baseQuery = database.Genres
                .AsNoTracking()
                .AsQueryable();

            if (hasNameFilter)
            {
                baseQuery = baseQuery.Where(genre => genre.NormalizedName.Contains(normalizedName));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var pageInfo = new StaticPagedList<int>(Array.Empty<int>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationGenre>(
                    Array.Empty<ApplicationGenre>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var genres = await baseQuery
                .OrderBy(genre => genre.Name)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var applicationGenres = genres
                .Select(genreMapper.ToApplicationGenre)
                .ToArray();

            return new PaginatedApplicationResponse<ApplicationGenre>(
                applicationGenres,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}