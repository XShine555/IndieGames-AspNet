using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record RetryGameArtworkCommand(
        string IdentityId,
        int GameId,
        int ArtworkId)
        : ICommand<Result>;
}
