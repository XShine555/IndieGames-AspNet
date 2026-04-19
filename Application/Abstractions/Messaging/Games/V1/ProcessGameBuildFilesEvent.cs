namespace Application.Abstractions.Messaging.Games.V1
{
    public record ProcessGameBuildFilesEvent(
        Guid GameId,
        Guid BuildId,
        string BuildStoragePath);
}
