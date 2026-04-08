using Application.Configuration;
using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class RemovePictureToGameCommandHandler(IDatabase database, IS3Service s3Service, ILogger<RemovePictureToGameCommandHandler> logger)
        : ICommandHandler<RemoveStorePictureToGameCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveStorePictureToGameCommand command, CancellationToken cancellationToken)
        {
            var picture = await database.GamePictures.AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == command.PictureId);
            if (picture is null)
            {
                logger.LogWarning("Picture with id {PictureId} not found", command.PictureId);
                return Result.NotFound();
            }

            if (picture.Game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of the game with id {GameId}", command.IdentityId, picture.GameId);
                return Result.Unauthorized();
            }

            try
            {
                await s3Service.RemoveFileAsync(picture.RelativePath + picture.Name, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error while removing picture with id {PictureId}", command.PictureId);
                return Result.Error("An error occurred while removing the picture");
            }

            database.GamePictures.Remove(picture);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}