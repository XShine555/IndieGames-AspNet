using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RetryGameArtworkCommand(
        Guid IdentityId,
        Guid GameId,
        Guid ArtworkId)
        : ICommand<Result>;
}
