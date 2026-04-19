namespace Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Models
{
    public record GameBuildFileMetadata(
        string FileRelativePath,
        string FileName,
        string FileContentType,
        long FileSize,
        string Hash,
        string HashAlgorithm);
}
