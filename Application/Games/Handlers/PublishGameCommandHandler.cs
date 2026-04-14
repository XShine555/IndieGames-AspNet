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
    public class PublishGameCommandHandler(
        IDatabase database,
        IGameMapper gameMapper,
        ILogger<PublishGameCommandHandler> logger)
        : ICommandHandler<PublishGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(PublishGameCommand command, CancellationToken cancellationToken)
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

            if (!artworksReady || !storePicturesReady)
            {
                logger.LogWarning("Game with id {GameId} cannot be published because not all artworks or store pictures are completed", game.Id);
                return Result.Invalid(new ValidationError("Game cannot be published until all artworks and store pictures are completed."));
            }

            game.IsPublished = true;
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(gameMapper.ToApplicationGame(game));
        }
    }
}
