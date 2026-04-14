using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserGameCollectionCommand(
        string UserId,
        int CollectionId,
        string Name)
        : ICommand<Result>;
}
