namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments
{
    public record DownloadFileFromBucketArguments(
        string Key,
        string DestinationFilePathVariable);
}
