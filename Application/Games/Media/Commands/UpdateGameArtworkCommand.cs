using Application.Abstractions.Common;
using Application.Games.Media.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Media.Commands
{
    public record UpdateGameArtworkCommand(
        Guid IdentityId,
        Guid GameId,
        Guid ArtworkId,
        IFileData FileData)
        : ICommand<Result<ApplicationGameArtwork>>;
}
