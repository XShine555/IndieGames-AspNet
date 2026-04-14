using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class UpdateGameCommandHandler(
        IDatabase database,
        IGameMapper gameMapper,
        ILogger<UpdateGameCommandHandler> logger)
        : ICommandHandler<UpdateGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for update", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId} and cannot update it", command.IdentityId, command.GameId);
                return Result.Forbidden();
            }

            var titleResult = await UpdateTitle(command.Title, game, cancellationToken);
            if (!titleResult.IsSuccess)
                return titleResult;

            UpdateDescription(command.Description, game);
            UpdateIsPublic(command.IsPublic, game);

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success(gameMapper.ToApplicationGame(game));
        }

        private void UpdateIsPublic(bool? isPublic, Game game)
        {
            if (!isPublic.HasValue)
                return;

            game.IsPublic = isPublic.Value;
        }

        private async Task<Result> UpdateTitle(string? newTitle, Game game, CancellationToken cancellationToken)
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

        private void UpdateDescription(string? newDescription, Game game)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                return;

            logger.LogInformation("Updating game description for game with id {GameId}", game.Id);
            game.Description = newDescription;
        }
    }
}
