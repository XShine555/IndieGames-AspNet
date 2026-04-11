namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments
{
    public record ResizePictureLocalArguments(
        string SourceFilePathVariable,
        string DestinationFilePathVariable,
        int Width,
        int Height);
}
