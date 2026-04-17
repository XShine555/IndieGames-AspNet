using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record PromoteUserToDeveloperCommand(Guid IdentityId)
        : ICommand<Result<ApplicationUserMutation>>;
}
