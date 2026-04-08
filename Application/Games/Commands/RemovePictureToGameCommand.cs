namespace Application.Games.Commands
{
    public record RemovePictureToGameCommand(
        int GameId,
        int PictureId);
}