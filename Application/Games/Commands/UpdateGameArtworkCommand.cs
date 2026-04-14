using Application.Abstractions.Common;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameArtworkCommand(
        string IdentityId,
        int GameId,
        int ArtworkId,
        IFileData FileData)
        : ICommand<Result<ApplicationGameArtwork>>;
}
