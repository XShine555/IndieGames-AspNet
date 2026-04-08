using Application.Configuration;
using Application.Contracts.Application;
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
        GameConfiguration gameConfiguration, IGamePicturesHelper gamePicturesHelper)
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

            string pictureKey = Guid.NewGuid() + command.fileData.FileExtension;
            string picturePath = Path.Combine(
                gameConfiguration.Routes.GetStorePictureFolderPath(command.GameId),
                pictureKey);
            try
            {
                await s3Service.UploadFileAsync(command.fileData, picturePath, cancellationToken);
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
                await s3Service.RemoveFileAsync(picturePath, cancellationToken);
                return Result.Error("Error saving picture");
            }

            var pictures = await gamePicturesHelper.GetPictures(game, cancellationToken);
            return Result.Success(ApplicationGame.FromEntity(game, pictures));
        }
    }
}