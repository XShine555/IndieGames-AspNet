namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments
{
    public record SynchronizeGamePicturesArguments(
        int PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable);
}
