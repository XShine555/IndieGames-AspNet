namespace Infrastructure.MassTransit.Arguments
{
    public record DownloadFileFromBucketArguments(
        string Key,
        string DestinationFilePathVariable);
}