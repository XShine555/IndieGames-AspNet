using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record CreateUserGameCollectionCommand(
        Guid UserId,
        string Name)
        : ICommand<Result<ApplicationUserCollection>>;
}
