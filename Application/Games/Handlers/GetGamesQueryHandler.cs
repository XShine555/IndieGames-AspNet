using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Games.Handlers
{
    public class GetGamesQueryHandler(
        IDatabase database,
        IGameMapper gameMapper)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
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

            var pageInfo = new StaticPagedList<int>(Array.Empty<int>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationGame>(
                    Array.Empty<ApplicationGame>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var pagedGameIds = await baseQuery
                .OrderByDescending(g => g.CreatedAt)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(g => g.Id)
                .ToListAsync(cancellationToken);

            var games = await database.Games
                .AsNoTracking()
                .AsSplitQuery()
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .Where(g => pagedGameIds.Contains(g.Id))
                .ToListAsync(cancellationToken);

            var gamesById = games.ToDictionary(game => game.Id);
            var orderedGames = pagedGameIds
                .Select(gameId => gamesById[gameId])
                .Select(gameMapper.ToApplicationGame)
                .ToArray();

            return new PaginatedApplicationResponse<ApplicationGame>(
                orderedGames,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}