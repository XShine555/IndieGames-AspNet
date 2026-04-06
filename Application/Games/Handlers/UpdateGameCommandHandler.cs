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
                return Result.Conflict();
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
    }
}