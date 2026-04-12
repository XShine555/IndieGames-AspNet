using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string OriginalPictureKey,
        string? SmallPictureKey,
        string? MediumPictureKey,
        string? LargePictureKey,
        string ProcessingStatus,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GameStorePictures gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                BuildPictureKey(gamePicture.OriginalRelativePath, gamePicture.OriginalName),
                BuildPictureKey(gamePicture.SmallRelativePath, gamePicture.SmallName),
                BuildPictureKey(gamePicture.MediumRelativePath, gamePicture.MediumName),
                BuildPictureKey(gamePicture.LargeRelativePath, gamePicture.LargeName),
                gamePicture.ProcessingStatus.ToString(),
                gamePicture.AddedAt);
        }

        static string BuildPictureKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;
            return $"{relativePath}/{name}";
        }
    }
}