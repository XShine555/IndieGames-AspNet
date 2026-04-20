using Application.Games.Media.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IGameMediaMapper
    {
        ApplicationGamePicture ToApplicationGamePicture(GameStorePictures gamePicture);

        ApplicationGameArtwork ToApplicationGameArtwork(GameArtwork artwork);
    }
}
