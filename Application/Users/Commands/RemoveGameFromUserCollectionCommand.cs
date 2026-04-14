using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record RemoveGameFromUserCollectionCommand(
        Guid UserId,
        Guid CollectionId,
        Guid GameId)
        : ICommand<Result>;
}
