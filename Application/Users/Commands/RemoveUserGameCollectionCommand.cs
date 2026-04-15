using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record RemoveUserGameCollectionCommand(
        Guid UserId,
        Guid CollectionId)
        : ICommand<Result>;
}
