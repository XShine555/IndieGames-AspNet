using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Queries;
using Application.Games.Catalog.Responses;
using Application.Users.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Games.Catalog.Handlers
{
    public class GetGamesQueryHandler(
        IDatabase database,
        IGameMediaMapper gameMediaMapper,
        IGenreMapper genreMapper)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGameListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGameListItem>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
        {
            var baseQuery = database.Games
                .AsNoTracking()
                .AsQueryable();

            switch (query.Mode)
            {
                case GameCatalogQueryMode.User:
                    baseQuery = baseQuery.Where(g => g.IsPublished);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var normalizedTitle = query.Title.Trim().ToUpperInvariant();
                baseQuery = baseQuery.Where(g => g.NormalizedTitle.Contains(normalizedTitle));
            }

            if (query.Genres?.Count > 0)
            {
                baseQuery = baseQuery.Where(g => g.Genres.Any(gg => query.Genres.Contains(gg.Id)));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var pageInfo = new StaticPagedList<Guid>(Array.Empty<Guid>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationGameListItem>(
                    Array.Empty<ApplicationGameListItem>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var games = await baseQuery
                .OrderByDescending(g => g.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(g => new ApplicationGameListItem(
                    g.Id,
                    g.Title,
                    g.Price,
                    g.Discount,
                    g.IsPublic,
                    g.IsPublished,
                    new ApplicationUserMutation(
                        g.Owner.IdentityId,
                        g.Owner.Username,
                        g.Owner.DisplayUsername,
                        g.Owner.Role,
                        g.Owner.UpdatedAt),
                    g.Genres.AsQueryable().Select(genreMapper.ToApplicationGenreFunction).ToList(),
                    g.Artworks.AsQueryable().Select(gameMediaMapper.ToApplicationGameArtworkFunction).ToList(),
                    g.CreatedAt,
                    g.UpdatedAt))
                .ToArrayAsync(cancellationToken);

            return new PaginatedApplicationResponse<ApplicationGameListItem>(
                games,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}
