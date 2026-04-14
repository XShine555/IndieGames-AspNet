using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record DeleteUserGameCollectionCommand(
        string UserId,
        int CollectionId)
        : ICommand<Result>;
}
