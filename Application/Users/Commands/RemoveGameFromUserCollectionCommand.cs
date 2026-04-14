using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record RemoveGameFromUserCollectionCommand(
        string UserId,
        int CollectionId,
        int GameId)
        : ICommand<Result>;
}
