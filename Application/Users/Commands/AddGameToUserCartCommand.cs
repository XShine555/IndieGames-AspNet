using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserCartCommand(
        Guid UserId,
        Guid GameId)
        : ICommand<Result>;
}
