namespace Application.Games.Builds.Responses
{
    public record ApplicationFileInfo(
        Guid Id,
        Guid GameBuildId,
        string FilePath,
        long Size,
        string Hash,
        string HashAlgorithm);
}
