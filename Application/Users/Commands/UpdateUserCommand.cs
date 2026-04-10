using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record UpdateUserCommand(
        string IdentityId,
        string NewDisplayUsername)
        : ICommand<Result<ApplicationUser>>;
}