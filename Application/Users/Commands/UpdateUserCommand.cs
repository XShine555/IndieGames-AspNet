using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserCommand(
        Guid IdentityId,
        string NewDisplayUsername)
        : ICommand<Result<ApplicationUserMutation>>;
}