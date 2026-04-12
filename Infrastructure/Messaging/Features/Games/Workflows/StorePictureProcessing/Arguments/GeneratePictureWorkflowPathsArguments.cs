namespace Infrastructure.Messaging.Features.Games.Workflows.StorePictureProcessing.Arguments
{
    public record GeneratePictureWorkflowPathsArguments(
        string TemporaryDirectory,
        string SourceKey);
}
