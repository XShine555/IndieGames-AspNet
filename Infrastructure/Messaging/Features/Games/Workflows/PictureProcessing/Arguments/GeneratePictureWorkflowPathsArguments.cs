namespace Infrastructure.Messaging.Features.Games.Workflows.PictureProcessing.Arguments
{
    public record GeneratePictureWorkflowPathsArguments(
        string TemporaryDirectory,
        string SourceKey);
}
