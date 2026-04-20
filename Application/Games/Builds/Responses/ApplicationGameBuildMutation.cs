namespace Application.Games.Builds.Responses
{
    public record ApplicationGameBuildMutation(
        Guid BuildId,
        string VersionName,
        DateTime CreatedAt);
}
