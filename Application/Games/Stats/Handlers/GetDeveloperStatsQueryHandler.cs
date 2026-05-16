using Application.Abstractions.Persistence;
using Application.Games.Stats.Queries;
using Application.Games.Stats.Responses;
using Ardalis.Result;
using Domain.Users.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Games.Stats.Handlers
{
    public class GetDeveloperStatsQueryHandler(IDatabase database)
        : IQueryHandler<GetDeveloperStatsQuery, Result<DeveloperStatsResponse>>
    {
        public async ValueTask<Result<DeveloperStatsResponse>> Handle(
            GetDeveloperStatsQuery query,
            CancellationToken cancellationToken)
        {
            var user = await database.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdentityId == query.DeveloperId, cancellationToken);

            if (user is null)
                return Result.NotFound("Usuario no encontrado.");

            if (user.Role != UserRole.Developer)
                return Result.Unauthorized("El usuario no tiene permisos de desarrollador.");

            var startOfMonth = new DateTime(
                DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var developerGameIds = await database.Games.AsNoTracking()
                .Where(g => g.OwnerId == query.DeveloperId)
                .Select(g => g.Id)
                .ToListAsync(cancellationToken);

            var purchases = await database.UserLibrary.AsNoTracking()
                .Where(ug => developerGameIds.Contains(ug.GameId))
                .Select(ug => new { ug.UserId, ug.purchasedAt })
                .ToListAsync(cancellationToken);

            var games = await database.Games.AsNoTracking()
                .Where(g => g.OwnerId == query.DeveloperId)
                .Select(g => new { g.IsPublished, g.ReleaseGameBuildId, g.CreatedAt })
                .ToListAsync(cancellationToken);

            var gamesSold = purchases.Count;
            var gamesSoldThisMonth = purchases.Count(p => p.purchasedAt >= startOfMonth);

            var players = purchases.Select(p => p.UserId).Distinct().Count();
            var playersThisMonth = purchases
                .Where(p => p.purchasedAt >= startOfMonth)
                .Select(p => p.UserId)
                .Distinct()
                .Count();

            var publishedGames = games.Count(g => g.IsPublished);
            var publishedThisMonth = games.Count(g => g.IsPublished && g.CreatedAt >= startOfMonth);

            var gamesWithIssues = games.Count(g => g.IsPublished && g.ReleaseGameBuildId is null);

            static string Subtitle(int thisMonth) =>
                thisMonth > 0 ? $"+{thisMonth} este mes" : "Sin cambios este mes";

            return Result.Success(new DeveloperStatsResponse(
                gamesSold,        Subtitle(gamesSoldThisMonth),
                players,          Subtitle(playersThisMonth),
                publishedGames,   Subtitle(publishedThisMonth),
                gamesWithIssues,  "Sin cambios este mes"));
        }
    }
}
