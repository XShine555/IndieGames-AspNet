using Application.Abstractions.Common;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddStorePictureToGameCommand(
        string IdentityId,
        int GameId,
        IFileData fileData)
        : ICommand<Result<ApplicationGame>>;
}
