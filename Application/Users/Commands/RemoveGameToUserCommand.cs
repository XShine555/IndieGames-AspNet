using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record RemoveGameToUserCommand(
        Guid UserId,
        Guid GameId)
        : ICommand<Result>;
}