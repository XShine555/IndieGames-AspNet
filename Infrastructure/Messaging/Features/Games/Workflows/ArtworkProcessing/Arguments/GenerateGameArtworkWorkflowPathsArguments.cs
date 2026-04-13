namespace Infrastructure.Messaging.Features.Games.Workflows.ArtworkProcessing.Arguments
{
    public record GenerateGameArtworkWorkflowPathsArguments(
        string TemporaryDirectory,
        string SourceKey);
}
