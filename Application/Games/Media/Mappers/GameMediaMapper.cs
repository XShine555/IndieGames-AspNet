using Application.Abstractions.Common;
using Application.Games.Media.Responses;
using Domain.Entities;

namespace Application.Games.Media.Mappers
{
    public class GameMediaMapper : IGameMediaMapper
    {
        public ApplicationGamePicture ToApplicationGamePicture(GameStorePictures gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                BuildKey(gamePicture.OriginalRelativePath, gamePicture.OriginalName),
                BuildKey(gamePicture.SmallRelativePath, gamePicture.SmallName),
                BuildKey(gamePicture.MediumRelativePath, gamePicture.MediumName),
                BuildKey(gamePicture.LargeRelativePath, gamePicture.LargeName),
                gamePicture.ProcessingStatus,
                gamePicture.AddedAt);
        }

        public ApplicationGameArtwork ToApplicationGameArtwork(GameArtwork artwork)
        {
            return new ApplicationGameArtwork(
                artwork.Id,
                artwork.Type,
                BuildKey(artwork.OriginalRelativePath, artwork.OriginalFileName),
                BuildKey(artwork.SmallRelativePath, artwork.SmallFileName),
                BuildKey(artwork.MediumRelativePath, artwork.MediumFileName),
                BuildKey(artwork.LargeRelativePath, artwork.LargeFileName),
                artwork.ProcessingStatus,
                artwork.CreatedAt,
                artwork.UpdatedAt);
        }

        private static string BuildKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return $"{relativePath}/{name}";
        }
    }
}
