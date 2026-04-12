namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record ResizePictureLocalArguments(
        string SourceFilePathVariable,
        string DestinationFilePathVariable,
        int Width,
        int Height);
}
