namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Logs
{
    public record ProcessAchievementPictureLog(
        Guid AchievementId,
        string SmallDestinationKey,
        string MediumDestinationKey,
        string LargeDestinationKey);
}
