namespace Infrastructure.MassTransit.Logs
{
    public record GenerateGamePicturesLog(
       int GameId,
       string OriginalPictureKey,
       string SmallPictureVariable,
       string MediumPictureVariable,
       string LargePictureVariable);
}