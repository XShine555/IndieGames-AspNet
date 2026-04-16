using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Application.Users.Responses;
using Domain.Entities;
using Domain.Users.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Games.Handlers
{
    public class GetGamesQueryHandler(IDatabase database, IGameMapper gameMapper, IGenreMapper genreMapper)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGameListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGameListItem>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
        {
            var normalizedTitle = query.Title.Trim().ToUpperInvariant();
            var hasTitleFilter = !string.IsNullOrWhiteSpace(normalizedTitle);
            var hasGenresFilter = query.Genres.Count > 0;

            var baseQuery = database.Games
                .AsNoTracking()
                .AsQueryable();

            if (query.ReadyOnly)
            {
                baseQuery = baseQuery.Where(g => g.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore);
            }
            if (query.OnlyPublished)
            {
                baseQuery = baseQuery.Where(g => g.IsPublished);
            }

            if (hasTitleFilter || hasGenresFilter)
            {
                baseQuery = baseQuery.Where(g =>
                    (hasTitleFilter && g.NormalizedTitle.Contains(normalizedTitle))
                    || (hasGenresFilter && g.Genres.Any(gg => query.Genres.Contains(gg.Id))));
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
                    g.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore,
                    g.IsPublic,
                    g.IsPublished,
                    new ApplicationUserMutation(
                        g.Owner.IdentityId,
                        g.Owner.Username,
                        g.Owner.DisplayUsername,
                        g.Owner.Role,
                        g.Owner.UpdatedAt),
                    g.Genres.Select(genreMapper.ToApplicationGenre).ToList(),
                    g.Artworks.Select(gameMapper.ToApplicationGameArtwork).ToList(),
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