namespace Infrastructure.MassTransit.Arguments
{
    public record GeneratePictureWorkflowPathsArguments(
        string TemporaryDirectory,
        string SourceKey);
}