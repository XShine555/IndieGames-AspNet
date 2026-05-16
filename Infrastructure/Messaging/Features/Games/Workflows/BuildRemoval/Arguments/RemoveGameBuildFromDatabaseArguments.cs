namespace Infrastructure.Messaging.Features.Games.Workflows.BuildRemoval.Arguments
{
    public record RemoveGameBuildFromDatabaseArguments(
        Guid GameId,
        Guid BuildId);
}
