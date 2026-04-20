using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record CompleteGameBuildCommand(
        Guid UserId,
        Guid BuildId)
        : ICommand<Result>;
}
