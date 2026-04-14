using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserCollectionCommand(
        string UserId,
        int CollectionId,
        int GameId)
        : ICommand<Result>;
}
