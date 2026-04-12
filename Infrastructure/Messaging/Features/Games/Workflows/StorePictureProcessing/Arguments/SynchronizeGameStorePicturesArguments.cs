namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record SynchronizeGameStorePicturesArguments(
        int PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable);
}
