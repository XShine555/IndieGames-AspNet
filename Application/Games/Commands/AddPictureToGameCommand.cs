using Application.Contracts.Application;

namespace Application.Games.Commands
{
    public record AddPictureToGameCommand(
        int GameId,
        IFileData fileData);
}