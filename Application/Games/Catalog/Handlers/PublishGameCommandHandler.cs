using Application.Abstractions.Persistence;
using Application.Games.Catalog.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Catalog.Handlers
{
    public class PublishGameCommandHandler(
        IDatabase database,
        ILogger<PublishGameCommandHandler> logger)
        : ICommandHandler<PublishGameCommand, Result>
    {
        public async ValueTask<Result> Handle(PublishGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .Include(g => g.Owner)
                .ThenInclude(o => o.OwnedGames)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);

            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for publish", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId} and cannot publish it", command.IdentityId, command.GameId);
                return Result.Forbidden();
            }

            if (game.IsPublished)
            {
                logger.LogWarning("Game with id {GameId} is already published and cannot be changed", game.Id);
                return Result.Conflict("Game is already published");
            }

            var publicationPendingItems = GetPublicationPendingItems(game);
            if (publicationPendingItems.Count > 0)
            {
                logger.LogWarning("Game with id {GameId} cannot be published because there are pending requirements", game.Id);
                return Result.Invalid(publicationPendingItems.Select(item => new ValidationError(item)).ToList());
            }

            game.Owner.OwnedGames.Add(new UserOwnedGame
            {
                UserId = game.Id,
                GameId = game.Id,
            });
            game.IsPublished = true;

            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("User with id {IdentityId} is publishing game with id {GameId}", command.IdentityId, command.GameId);

            return Result.Success();
        }

        private static List<string> GetPublicationPendingItems(Game game)
        {
            var pendingItems = new List<string>();

            if (game.Artworks.Count == 0)
            {
                pendingItems.Add("Add at least one artwork.");
            }
            else if (game.Artworks.Any(a => a.ProcessingStatus != GameArtworkProcessingStatus.Completed))
            {
                pendingItems.Add("Complete processing for all artworks.");
            }

            if (game.StorePictures.Count == 0)
            {
                pendingItems.Add("Add at least one store picture.");
            }
            else if (game.StorePictures.Any(p => p.ProcessingStatus != GamePictureProcessingStatus.Completed))
            {
                pendingItems.Add("Complete processing for all store pictures.");
            }

            if (!game.ReleaseGameBuildId.HasValue)
            {
                pendingItems.Add("Assign a release build.");
            }

            return pendingItems;
        }
    }
}
