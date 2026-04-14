using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserLibraryCommand(
        string UserId,
        int GameId)
        : ICommand<Result>;
}
