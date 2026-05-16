using Application.Games.Media.Responses;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions.Common
{
    public interface IGameMediaMapper
    {
        ApplicationGamePicture ToApplicationGamePicture(GameStorePictures gamePicture);

        Expression<Func<GameArtwork, ApplicationGameArtwork>> ToApplicationGameArtworkFunction { get; }

        ApplicationGameArtwork ToApplicationGameArtwork(GameArtwork artwork);
    }
}
