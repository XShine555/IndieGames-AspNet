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

            var artworksReady = game.Artworks.Count > 0
                && game.Artworks.All(a => a.ProcessingStatus == GameArtworkProcessingStatus.Completed);
            var storePicturesReady = game.StorePictures.Count > 0
                && game.StorePictures.All(p => p.ProcessingStatus == GamePictureProcessingStatus.Completed);
            var hasReleaseBuild = game.ReleaseGameBuildId.HasValue;

            if (!artworksReady || !storePicturesReady || !hasReleaseBuild)
            {
                logger.LogWarning("Game with id {GameId} cannot be published because not all artworks, store pictures, or release build are completed", game.Id);
                return Result.Invalid(new ValidationError("Game cannot be published until all artworks, store pictures, and release build are completed."));
            }

            game.IsPublished = true;
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
