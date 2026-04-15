using Application.Games.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IGameMapper
    {
        ApplicationGame ToApplicationGame(Game game);

        ApplicationGamePicture ToApplicationGamePicture(GameStorePictures gamePicture);

        ApplicationGameArtwork ToApplicationGameArtwork(GameArtwork artwork);

        ApplicationGameMutation ToApplicationGameMutation(Game game);

        ApplicationGameGenresMutation ToApplicationGameGenresMutation(Game game);
    }
}
