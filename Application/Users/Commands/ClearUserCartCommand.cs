using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record ClearUserCartCommand(Guid UserId) : ICommand<Result>;
}
