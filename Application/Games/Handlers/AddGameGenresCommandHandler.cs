using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class AddGameGenresCommandHandler(
        IDatabase database,
        ILogger<AddGameGenresCommandHandler> logger)
        : ICommandHandler<AddGameGenresCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(AddGameGenresCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for genre addition", command.GameId);
                return Result.NotFound();
            }

            var requestedGenreIds = command.Genres.ToArray();
            if (requestedGenreIds.Length != requestedGenreIds.Distinct().Count())
            {
                logger.LogWarning("Duplicate genre ids were provided for game with id {GameId}", command.GameId);
                return Result.Conflict("Duplicate genre ids are not allowed");
            }

            var existingGenres = await database.Genres
                .AsNoTracking()
                .Where(g => requestedGenreIds.Contains(g.Id))
                .ToArrayAsync(cancellationToken);

            var missingGenreIds = requestedGenreIds.Except(existingGenres.Select(g => g.Id)).ToArray();
            if (missingGenreIds.Length > 0)
            {
                logger.LogWarning(
                    "Some genres were not found when adding to game with id {GameId}. Missing genres: {MissingGenreIds}",
                    command.GameId,
                    string.Join(", ", missingGenreIds));
                return Result.NotFound($"The following genres were not found: {string.Join(", ", missingGenreIds)}");
            }

            await database.Games
                .Where(g => g.Id == game.Id)
                .Include(g => g.Genres)
                .LoadAsync(cancellationToken);

            var alreadyAssociatedGenres = game.Genres.Where(g => requestedGenreIds.Contains(g.Id)).ToArray();
            if (alreadyAssociatedGenres.Length > 0)
            {
                logger.LogWarning(
                    "Some genres are already associated with game id {GameId}: {GenreIds}",
                    command.GameId,
                    string.Join(", ", alreadyAssociatedGenres.Select(g => g.Id)));
                return Result.Conflict($"The following genres are already associated with the game: {string.Join(", ", alreadyAssociatedGenres.Select(g => g.Name))}");
            }

            foreach (var genre in existingGenres)
                game.Genres.Add(genre);

            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added genres for game with id {GameId}", command.GameId);
            return Result.Success(await LoadGameAsync(game.Id, cancellationToken));
        }

        async Task<ApplicationGame> LoadGameAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var updatedGame = await database.Games
                .Where(g => g.Id == gameId)
                .Include(g => g.Genres)
                .Include(g => g.UsersToGames).ThenInclude(utg => utg.User)
                .Include(g => g.UsersToGameRequests).ThenInclude(utgr => utgr.User)
                .SingleAsync(cancellationToken);

            return ApplicationGame.FromEntity(updatedGame);
        }
    }
}
