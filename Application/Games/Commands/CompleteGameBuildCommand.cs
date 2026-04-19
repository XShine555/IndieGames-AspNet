using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record CompleteGameBuildCommand(
        Guid UserId,
        Guid BuildId)
        : ICommand<Result>;
}