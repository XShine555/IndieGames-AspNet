using Domain.Games.Enums;

namespace Application.Games.Builds.Responses
{
    public record ApplicationGameBuildListItem(
        Guid BuildId,
        string VersionName,
        GameBuildStatus Status,
        bool IsReleaseBuild);
}