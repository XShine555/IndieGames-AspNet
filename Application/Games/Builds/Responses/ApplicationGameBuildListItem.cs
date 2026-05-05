using Domain.Games.Enums;

namespace Application.Games.Builds.Responses
{
    public record ApplicationGameBuildListItem(
        Guid BuildId,
        string VersioName,
        GameBuildStatus Status,
        bool IsReleaseBuild);
}