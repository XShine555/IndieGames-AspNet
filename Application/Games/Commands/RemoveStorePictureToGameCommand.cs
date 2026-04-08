using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveStorePictureToGameCommand(
        string IdentityId,
        int PictureId)
        : ICommand<Result>;
}