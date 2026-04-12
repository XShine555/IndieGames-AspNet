namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Logs
{
    public record DownloadFileFromBucketLog(
        string SourceKey,
        string DestinationFilePath);
}
