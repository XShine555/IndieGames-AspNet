using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record CreateUserCommand(
        Guid IdentityId,
        string Username)
        : ICommand<Result<ApplicationUser>>;
}