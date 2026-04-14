using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserLibraryCommand(
        Guid UserId,
        Guid GameId)
        : ICommand<Result>;
}
