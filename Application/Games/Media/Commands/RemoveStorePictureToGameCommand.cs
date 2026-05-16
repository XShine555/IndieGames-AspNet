using Ardalis.Result;
using Mediator;

namespace Application.Games.Media.Commands
{
    public record RemoveStorePictureToGameCommand(
        Guid IdentityId,
        Guid PictureId)
        : ICommand<Result>;
}
