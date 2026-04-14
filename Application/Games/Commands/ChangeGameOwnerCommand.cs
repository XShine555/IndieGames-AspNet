using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record ChangeGameOwnerCommand(
        Guid IdentityId,
        Guid GameId,
        Guid NewOwnerId)
        : ICommand<Result<ApplicationGame>>;
}
