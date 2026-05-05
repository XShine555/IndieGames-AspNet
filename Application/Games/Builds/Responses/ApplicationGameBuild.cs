using Domain.Games.Enums;

namespace Application.Games.Builds.Responses
{
    public record ApplicationGameBuild(
        Guid BuildId,
        string VersionName,
        string? ManifestPath,
        bool IsReleaseBuild,
        GameBuildStatus Status,
        DateTime CreatedAt);
}
