using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record CompleteGameBuildCommand(
        Guid BuildId,
        Guid UserId)
        : ICommand<Result>;
}