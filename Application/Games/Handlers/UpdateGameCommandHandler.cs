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
            var game = await database.Games.SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);
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

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success(ApplicationGame.FromEntity(game));
        }

        async Task<Result> UpdateTitle(string? newTitle, Game game, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return Result.Success();

            var trimmedTitle = newTitle.Trim();
            var normalizedTitle = trimmedTitle.ToUpperInvariant();
            var existingGame = await database.Games
                .SingleOrDefaultAsync(g => g.NormalizedTitle == normalizedTitle && g.Id != game.Id, cancellationToken);
            if (existingGame is not null)
            {
                logger.LogWarning("Game with title {GameTitle} already exists with id {ExistingGameId}", newTitle, existingGame.Id);
                return Result.Conflict("Another Game with the same title already exists");
            }

            game.Title = trimmedTitle;
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

        async Task<Result> UpdateGenres(ICollection<Guid> genres, Game game, CancellationToken cancellationToken)
        {
            var existingGenres = await database.Genres.AsNoTracking()
                .Where(g => genres.Contains(g.Id))
                .ToListAsync(cancellationToken);
            var notExistingGenres = genres.Except(existingGenres.Select(g => g.Id)).ToList();
            if (notExistingGenres.Count > 0)
            {
                logger.LogWarning("Genres with ids {GenreIds} not found for game with id {GameId}", notExistingGenres, game.Id);
                return Result.NotFound($"Genres with ids {string.Join(", ", notExistingGenres) } not found");
            }

            logger.LogInformation("Updating game genres for game with id {GameId}", game.Id);
            game.Genres = existingGenres;
            return Result.Success();
        }
    }
}