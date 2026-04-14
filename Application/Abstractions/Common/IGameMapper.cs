using Application.Games.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IGameMapper
    {
        ApplicationGame ToApplicationGame(Game game);
    }
}
