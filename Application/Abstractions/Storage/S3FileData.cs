namespace Application.Abstractions.Storage
{
    public record S3FileData(
        string FileName,
        string ContentType,
        long Length);
}