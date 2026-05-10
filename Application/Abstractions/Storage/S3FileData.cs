namespace Application.Abstractions.Storage
{
    public record S3FileData(
        string? FilePath,
        string FileName,
        string ContentType,
        long Length);
}