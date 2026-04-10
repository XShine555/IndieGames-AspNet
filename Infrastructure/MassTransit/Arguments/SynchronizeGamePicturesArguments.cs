namespace Infrastructure.MassTransit.Arguments
{
    public record SynchronizeGamePicturesArguments(
        int PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable);
}