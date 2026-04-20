using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveGameBuildCommand(
        Guid UserId,
        Guid BuildId)
        : ICommand<Result>;
}
