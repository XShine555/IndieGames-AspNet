using Domain.Games.Enums;

namespace Application.Games.Builds.Responses
{
    public record ApplicationGameBuild(
        Guid BuildId,
        string VersionName,
        GameBuildStatus Status,
        bool IsReleaseBuild,
        DateTime CreatedAt);
}
