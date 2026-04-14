using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record CreateUserGameCollectionCommand(
        string UserId,
        string Name)
        : ICommand<Result<int>>;
}
