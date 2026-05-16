using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Commands
{
    public record PublishGameCommand(
        Guid IdentityId,
        Guid GameId)
        : ICommand<Result>;
}
