using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RemoveGameCommand(
        Guid IdentityId,
        Guid Id)
        : ICommand<Result>;
}