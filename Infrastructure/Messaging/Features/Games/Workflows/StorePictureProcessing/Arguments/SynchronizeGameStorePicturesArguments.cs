namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record SynchronizeGameStorePicturesArguments(
        Guid PictureId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable);
}
