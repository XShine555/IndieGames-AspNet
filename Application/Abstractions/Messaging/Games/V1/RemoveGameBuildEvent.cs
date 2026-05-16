namespace Application.Abstractions.Messaging.Games.V1
{
    public record RemoveGameBuildEvent(
        Guid GameId,
        Guid BuildId,
        string BuildStoragePath);
}
