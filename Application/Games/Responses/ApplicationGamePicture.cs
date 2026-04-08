using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string OriginalName,
        string FileExtension,
        string RelativePath,
        string Name,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GameOriginalPicture gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                gamePicture.OriginalName,
                gamePicture.FileExtension,
                gamePicture.RelativePath,
                gamePicture.Name,
                gamePicture.AddedAt);
        }
    }
}