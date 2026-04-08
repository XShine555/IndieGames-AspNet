using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGamePicture(
        int PictureId,
        string PictureUrl,
        DateTime AddedAt)
    {
        public static ApplicationGamePicture FromEntity(GamePicture gamePicture, string pictureUrl)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                pictureUrl,
                gamePicture.AddedAt);
        }
    }
}