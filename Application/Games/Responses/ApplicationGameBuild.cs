using Domain.Games.Enums;

namespace Application.Games.Responses
{
    public record ApplicationGameBuild(
        Guid BuildId,
        string VersionName,
        GameBuildStatus Status,
        bool IsReleaseBuild,
        DateTime CreatedAt);
}
