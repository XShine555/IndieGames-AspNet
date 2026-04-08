using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Queries;
using Application.Games.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.EF;

namespace Application.Games.Handlers
{
    public class GetGamesQueryHandler(IDatabase database, IGamePicturesHelper gamePicturesHelper)
        : IQueryHandler<GetGamesQuery, PaginatedApplicationResponse<ApplicationGame>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationGame>> Handle(GetGamesQuery query, CancellationToken cancellationToken)
        {
            var normalizedTitle = query.Title.Trim().ToLower();
            var totalCount = await database.Games.CountAsync(cancellationToken);
            var pagedGames = await database.Games.AsNoTracking()
                .Include(g => g.Genres)
                .Include(g => g.Pictures)
                .Where(g => g.NormalizedTitle.Contains(normalizedTitle)
                    || g.Genres.Any(gg => query.Genres.Contains(gg.Id)))
                .ToPagedListAsync(query.PageNumber, query.PageSize, totalCount, cancellationToken);

            var applicationGames = new List<ApplicationGame>(pagedGames.Count);
            foreach (var game in pagedGames)
            {
                var pictures = await gamePicturesHelper.GetPictures(game, cancellationToken);
                applicationGames.Add(ApplicationGame.FromEntity(game, pictures));
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