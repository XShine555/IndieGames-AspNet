using Application.Abstractions.Common;
using Application.Games.Media.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Media.Commands
{
    public record AddStorePictureToGameCommand(
        Guid IdentityId,
        Guid GameId,
        IFileData fileData)
        : ICommand<Result<ApplicationGamePicture>>;
}
