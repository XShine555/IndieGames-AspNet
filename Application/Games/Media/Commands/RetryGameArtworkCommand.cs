using Ardalis.Result;
using Mediator;

namespace Application.Games.Media.Commands
{
    public record RetryGameArtworkCommand(
        Guid IdentityId,
        Guid GameId,
        Guid ArtworkId)
        : ICommand<Result>;
}
