namespace Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments
{
    public record GenerateGameBuildManifestArguments(
        Guid GameId,
        Guid BuildId,
        string BuildStoragePath);
}
