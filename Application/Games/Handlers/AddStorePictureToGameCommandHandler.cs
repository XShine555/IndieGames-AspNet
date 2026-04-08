using Application.Configuration;
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
    public class AddStorePictureToGameCommandHandler(IDatabase database, IS3Service s3Service, ILogger<AddStorePictureToGameCommandHandler> logger,
        GameConfiguration gameConfiguration)
        : ICommandHandler<AddStorePictureToGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(AddStorePictureToGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId}", command.IdentityId, command.GameId);
                return Result.Unauthorized();
            }

            string pictureName = Guid.NewGuid() + command.fileData.FileExtension;
            string pictureKey = gameConfiguration.Routes.BuildStorePicturePath(game.Id, pictureName);
            try
            {
                await s3Service.UploadFileAsync(command.fileData, pictureKey, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error uploading picture for game with id {GameId}", command.GameId);
                return Result.Error("Error uploading picture");
            }

            var newPicture = new GamePicture
            {
                GameId = command.GameId,
                PictureKey = pictureKey
            };
            await database.GamePictures.AddAsync(newPicture, cancellationToken);
            try
            {
                await database.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error saving picture for game with id {GameId}", command.GameId);
                await s3Service.RemoveFileAsync(pictureKey, cancellationToken);
                return Result.Error("Error saving picture");
            }

            return Result.Success(ApplicationGame.FromEntity(game));
        }
    }
}