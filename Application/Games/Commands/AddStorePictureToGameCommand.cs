using Application.Contracts.Application;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddStorePictureToGameCommand(
        int GameId,
        IFileData fileData)
        : ICommand<Result<ApplicationGame>>;
}