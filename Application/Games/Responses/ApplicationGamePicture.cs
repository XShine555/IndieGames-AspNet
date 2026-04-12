using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string OriginalPictureKey,
        string SmallPictureKey,
        string MediumPictureKey,
        string LargePictureKey,
        string ProcessingStatus,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GameStorePictures gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                gamePicture.OriginalRelativePath + gamePicture.OriginalName,
                gamePicture.SmallRelativePath + gamePicture.SmallName,
                gamePicture.MediumRelativePath + gamePicture.MediumName,
                gamePicture.LargeRelativePath + gamePicture.LargeName,
                gamePicture.ProcessingStatus.ToString(),
                gamePicture.AddedAt);
        }
    }
}