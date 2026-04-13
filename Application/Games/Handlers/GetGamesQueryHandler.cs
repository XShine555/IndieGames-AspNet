using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Domain.Entities;
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

            var gamesQuery = database.Games.AsNoTracking()
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Where(g => g.NormalizedTitle.Contains(normalizedTitle)
                    || g.Genres.Any(gg => query.Genres.Contains(gg.Id)));

            if (query.ReadyOnly)
            {
                gamesQuery = gamesQuery.Where(g => g.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore);
            }

            var totalCount = await gamesQuery.CountAsync(cancellationToken);
            var pagedGames = await gamesQuery
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            var applicationGames = new List<ApplicationGame>(pagedGames.Count);
            foreach (var game in pagedGames)
            {
                applicationGames.Add(ApplicationGame.FromEntity(game));
            }

            return new PaginatedApplicationResponse<ApplicationGame>(
                applicationGames,
                pagedGames.PageNumber,
                pagedGames.PageSize,
                pagedGames.PageCount,
                pagedGames.TotalItemCount,
                pagedGames.HasNextPage,
                pagedGames.HasPreviousPage);
        }
    }
}