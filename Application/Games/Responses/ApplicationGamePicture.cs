using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string PictureKey,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GamePicture gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                gamePicture.PictureKey,
                gamePicture.AddedAt);
        }
    }
}