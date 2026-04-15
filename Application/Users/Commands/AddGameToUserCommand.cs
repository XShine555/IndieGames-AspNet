using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserCommand(
        Guid UserId,
        Guid GameId)
        : ICommand<Result<ApplicationUserOwnedGame>>;
}