namespace Application.Games.Responses
{
    public record ApplicationGameBuildMutation(
        Guid BuildId,
        string VersionName,
        DateTime CreatedAt);
}