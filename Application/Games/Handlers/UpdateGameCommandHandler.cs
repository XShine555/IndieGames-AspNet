using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class UpdateGameCommandHandler(IDatabase database, ILogger<UpdateGameCommandHandler> logger)
        : ICommandHandler<UpdateGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.Id, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for update", command.Id);
                return Result.NotFound();
            }

            var titleResult = await UpdateTitle(command.Title, game, cancellationToken);
            if (!titleResult.IsSuccess)
                return titleResult;

            UpdateDescription(command.Description, game);
            var genresResult = await UpdateGenres(command.Genres, game, cancellationToken);
            if (!genresResult.IsSuccess)
                return genresResult;

            database.Games.Update(game);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        async Task<Result> UpdateTitle(string? newTitle, Game game, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return Result.Success();

            var normalizedTitle = newTitle.Trim().ToUpperInvariant();
            var existingGame = await database.Games
                .SingleOrDefaultAsync(g => g.NormalizedTitle == normalizedTitle, cancellationToken);
            if (existingGame is not null)
            {
                logger.LogWarning("Game with title {GameTitle} already exists with id {ExistingGameId}", newTitle, existingGame.Id);
                return Result.Conflict("Another Game with the same title already exists");
            }

            game.Title = newTitle;
            game.NormalizedTitle = normalizedTitle;

            logger.LogInformation("Updating game title for game with id {GameId}", game.Id);
            return Result.Success();
        }

        void UpdateDescription(string? newDescription, Game game)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                return;

            logger.LogInformation("Updating game description for game with id {GameId}", game.Id);
            game.Description = newDescription;
        }

        async Task<Result> UpdateGenres(ICollection<Guid>? genres, Game game, CancellationToken cancellationToken)
        {
            if (genres is null)
                return Result.Success();

            var repeatedGuids = game.Genres.Where(g => genres.Contains(g.Id)).ToArray();
            if (repeatedGuids.Length > 0)
                return Result.Conflict($"The following genres are already associated with the game: {string.Join(", ", repeatedGuids.Select(g => g.Name)) }");

            var existingGenres = await database.Genres.AsNoTracking()
                .Where(g => genres.Contains(g.Id))
                .ToArrayAsync(cancellationToken);
            var notExistings = genres.Except(existingGenres.Select(g => g.Id)).ToArray();
            if (notExistings.Length > 0)
                return Result.NotFound($"The following genres were not found: {string.Join(", ", notExistings) }");

            game.Genres = game.Genres.Union(existingGenres).ToArray();
            return Result.Success();
        }
    }
}