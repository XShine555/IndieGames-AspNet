namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Logs
{
    public record DownloadFileFromBucketLog(
        string Key,
        string DestinationFilePath);
}
