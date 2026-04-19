using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record CompleteGameBuildCommand(
        Guid GameId,
        Guid UserId)
        : ICommand<Result>;
}