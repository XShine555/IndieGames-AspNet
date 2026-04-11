namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments
{
    public record UploadFileToBucketArguments(
        string FilePathVariable,
        string DestinationRoute);
}
