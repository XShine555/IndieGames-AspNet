using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record DeleteUserGameCollectionCommand(
        Guid UserId,
        Guid CollectionId)
        : ICommand<Result>;
}
