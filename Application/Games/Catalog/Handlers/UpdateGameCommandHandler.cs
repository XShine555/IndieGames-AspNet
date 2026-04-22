using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Commands;
using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Domain.Entities;
using Domain.Games.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Catalog.Handlers
{
    public class UpdateGameCommandHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper,
        ILogger<UpdateGameCommandHandler> logger)
        : ICommandHandler<UpdateGameCommand, Result<ApplicationGameMutation>>
    {
        public async ValueTask<Result<ApplicationGameMutation>> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
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
                return Result.Conflict(titleResult.Errors.FirstOrDefault());

            UpdatePrice(command.Price, game);
            UpdateDiscount(command.Discount, game);
            UpdateDescription(command.Description, game);
            UpdateIsPublic(command.IsPublic, game);

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success(gameCatalogMapper.ToApplicationGameMutation(game));
        }

        private static void UpdateIsPublic(bool isPublic, Game game)
        {
            game.IsPublic = isPublic;
        }

        private static void UpdatePrice(decimal newPrice, Game game)
        {
            game.Price = newPrice;
        }

        private static void UpdateDiscount(decimal newDiscount, Game game)
        {
            game.Discount = newDiscount;
        }

        private async Task<Result> UpdateTitle(string newTitle, Game game, CancellationToken cancellationToken)
        {
            var trimmedTitle = newTitle.Trim();
            var normalizedTitle = trimmedTitle.ToUpperInvariant();
            var existingGame = await database.Games
                .AsNoTracking()
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

        private void UpdateDescription(string newDescription, Game game)
        {
            logger.LogInformation("Updating game description for game with id {GameId}", game.Id);
            game.Description = newDescription;
        }
    }
}
