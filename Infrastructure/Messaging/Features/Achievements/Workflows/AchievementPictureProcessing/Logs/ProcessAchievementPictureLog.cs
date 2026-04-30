namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Logs
{
    public record ProcessAchievementPictureLog(
        string SmallDestinationKey,
        string MediumDestinationKey,
        string LargeDestinationKey);
}
