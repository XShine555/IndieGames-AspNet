using Application.Configuration;
using Application.Contracts.Application;
using Application.Contracts.Infrastructure;
using Application.Games.Responses;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Games.Helpers
{
    public class GamePicturesHelper(IS3Service s3Service, ILogger<GamePicturesHelper> logger,
        GameConfiguration gameConfiguration) : IGamePicturesHelper
    {
        public async Task<IReadOnlyCollection<ApplicationGamePicture>> GetPictures(Game game, CancellationToken cancellationToken)
        {
            var pictures = new List<ApplicationGamePicture>();
            foreach (var picture in game.Pictures)
            {
                try
                {
                    var pictureKey = Path.Combine(
                        gameConfiguration.Routes.GetStorePictureFolderPath(game.Id),
                        picture.PictureKey);
                    var pictureUrl = await s3Service.GetSignedUrlAsync(pictureKey, TimeSpan.FromHours(1), cancellationToken);
                    pictures.Add(ApplicationGamePicture.FromEntity(picture, pictureUrl));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to get pre-signed URL for game picture with S3 key {S3Key}", picture.PictureKey);
                }
            }
            return pictures;
        }
    }
}