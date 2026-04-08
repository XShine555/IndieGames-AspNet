using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserCommand(
        string UserId,
        Guid GameId)
        : ICommand<Result<ApplicationUser>>;
}