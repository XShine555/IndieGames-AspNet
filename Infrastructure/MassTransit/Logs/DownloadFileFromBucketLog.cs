namespace Infrastructure.MassTransit.Logs
{
    public record DownloadFileFromBucketLog(
        string Key,
        string DestinationFilePath);
}