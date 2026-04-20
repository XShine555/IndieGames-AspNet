using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record RemoveGameBuildCommand(
        Guid UserId,
        Guid BuildId)
        : ICommand<Result>;
}
