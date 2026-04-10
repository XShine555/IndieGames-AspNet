namespace Infrastructure.MassTransit.Arguments
{
    public record UploadFileToBucketArguments(
        string FilePathVariable,
        string DestinationRoute);
}