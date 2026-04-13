namespace Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments
{
    public record SynchronizeGameArtworkArguments(
        int ArtworkId,
        string SmallPictureVariable,
        string MediumPictureVariable,
        string LargePictureVariable,
        string SmallRelativePath,
        string MediumRelativePath,
        string LargeRelativePath,
        int SmallWidth,
        int SmallHeight,
        int MediumWidth,
        int MediumHeight,
        int LargeWidth,
        int LargeHeight);
}
