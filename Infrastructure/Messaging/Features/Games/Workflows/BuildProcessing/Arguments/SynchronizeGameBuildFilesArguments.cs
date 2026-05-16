namespace Infrastructure.Messaging.Features.Games.Workflows.BuildProcessing.Arguments
{
    public record SynchronizeGameBuildFilesArguments(
        Guid GameId,
        Guid BuildId);
}
