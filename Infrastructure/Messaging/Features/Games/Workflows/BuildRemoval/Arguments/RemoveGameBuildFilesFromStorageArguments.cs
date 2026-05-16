namespace Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments
{
    public record RemoveGameBuildFilesFromStorageArguments(
        Guid GameId,
        Guid BuildId,
        string BuildStoragePath);
}
