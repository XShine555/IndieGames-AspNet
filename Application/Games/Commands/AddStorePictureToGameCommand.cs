using Application.Abstractions.Common;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddStorePictureToGameCommand(
        Guid IdentityId,
        Guid GameId,
        IFileData fileData)
        : ICommand<Result<ApplicationGamePicture>>;
}
