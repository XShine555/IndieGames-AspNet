namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record DownloadFileFromBucketArguments(
        string Key,
        string DestinationFilePathVariable);
}
