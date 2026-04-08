using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveStorePictureToGameCommand(
        int PictureId)
        : ICommand<Result>;
}