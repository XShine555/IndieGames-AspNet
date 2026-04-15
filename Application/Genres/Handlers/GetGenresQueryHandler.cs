using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Genres.Handlers
{
    public class GetGenresQueryHandler(IDatabase database)
        : IQueryHandler<GetGenresQuery, PaginatedApplicationResponse<ApplicationGenreListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGenreListItem>> Handle(GetGenresQuery query, CancellationToken cancellationToken)
        {
            var normalizedName = query.Name?.Trim().ToUpperInvariant();
            var hasNameFilter = !string.IsNullOrWhiteSpace(normalizedName);

            var baseQuery = database.Genres
                .AsNoTracking()
                .AsQueryable();

            if (hasNameFilter)
            {
                baseQuery = baseQuery.Where(genre => genre.NormalizedName.Contains(normalizedName!));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var pageInfo = new StaticPagedList<Guid>(Array.Empty<Guid>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationGenreListItem>(
                    Array.Empty<ApplicationGenreListItem>(),
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
                .Select(genre => new ApplicationGenreListItem(
                    genre.Id,
                    genre.Name,
                    genre.Games.Count,
                    genre.CreatedAt,
                    genre.UpdatedAt))
                .ToArrayAsync(cancellationToken);

            return new PaginatedApplicationResponse<ApplicationGenreListItem>(
                genres,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}