using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record AddGameToUserCollectionCommand(
        Guid UserId,
        Guid CollectionId,
        Guid GameId)
        : ICommand<Result>;
}
