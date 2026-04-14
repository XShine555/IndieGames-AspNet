using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RetryGameStorePictureCommand(
        string IdentityId,
        int GameId,
        int PictureId)
        : ICommand<Result>;
}
