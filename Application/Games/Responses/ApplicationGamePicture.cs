using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string OriginalName,
        string FileExtension,
        string RelativePath,
        string Name,
        string? SmallRelativePath,
        string? SmallName,
        string? SmallFileExtension,
        string? MediumRelativePath,
        string? MediumName,
        string? MediumFileExtension,
        string? LargeRelativePath,
        string? LargeName,
        string? LargeFileExtension,
        string ProcessingStatus,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GameStorePictures gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                gamePicture.OriginalName,
                gamePicture.ContentType,
                gamePicture.RelativePath,
                gamePicture.Name,
                gamePicture.SmallRelativePath,
                gamePicture.SmallName,
                gamePicture.SmallFileExtension,
                gamePicture.MediumRelativePath,
                gamePicture.MediumName,
                gamePicture.MediumFileExtension,
                gamePicture.LargeRelativePath,
                gamePicture.LargeName,
                gamePicture.LargeFileExtension,
                gamePicture.ProcessingStatus.ToString(),
                gamePicture.AddedAt);
        }
    }
}