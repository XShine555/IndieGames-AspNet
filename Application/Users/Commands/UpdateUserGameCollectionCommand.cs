using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserGameCollectionCommand(
        Guid UserId,
        Guid CollectionId,
        string Name)
        : ICommand<Result>;
}
