using Application.Games.Responses;
using Domain.Entities;

namespace Application.Contracts.Application
{
    public interface IGamePicturesHelper
    {
        Task<IReadOnlyCollection<ApplicationGamePicture>> GetPictures(Game game, CancellationToken cancellationToken);
    }
}