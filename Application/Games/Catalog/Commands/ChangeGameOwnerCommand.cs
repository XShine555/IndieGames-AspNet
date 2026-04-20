using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Commands
{
    public record ChangeGameOwnerCommand(
        Guid IdentityId,
        Guid GameId,
        Guid NewOwnerId)
        : ICommand<Result<ApplicationGameMutation>>;
}
