using Ardalis.Result;
using Mediator;

namespace Application.Users.Commands
{
    public record RemoveGameFromUserCartCommand(
        Guid UserId,
        Guid GameId)
        : ICommand<Result>;
}
