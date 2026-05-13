using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record FulfillCartOrderCommand(
        Guid UserId,
        IReadOnlyCollection<Guid> GameIds)
        : ICommand<Result>;
}
