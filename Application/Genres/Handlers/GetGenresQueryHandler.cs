using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

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

            var genres = await baseQuery
                .OrderBy(genre => genre.Name)
                .Select(genre => new ApplicationGenreListItem(
                    genre.Id,
                    genre.Name,
                    genre.Games.Count,
                    genre.CreatedAt,
                    genre.UpdatedAt))
                .ToListAsync(cancellationToken);

            var pagedList = genres.ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationGenreListItem>.FromPagedList(pagedList);
        }
    }
}