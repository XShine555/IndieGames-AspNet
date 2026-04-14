using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RetryGameStorePictureCommand(
        Guid IdentityId,
        Guid GameId,
        Guid PictureId)
        : ICommand<Result>;
}
