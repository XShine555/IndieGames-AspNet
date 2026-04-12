namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record UploadFileToBucketArguments(
        string FilePathVariable,
        string DestinationRoute);
}
