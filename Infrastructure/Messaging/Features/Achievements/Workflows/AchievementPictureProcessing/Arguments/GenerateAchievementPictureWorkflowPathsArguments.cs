namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments
{
    public record GenerateAchievementPictureWorkflowPathsArguments(
        string TemporaryDirectory,
        string SourceKey);
}
