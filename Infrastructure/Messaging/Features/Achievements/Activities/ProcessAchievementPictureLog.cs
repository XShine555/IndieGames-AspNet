namespace Infrastructure.Messaging.Features.Achievements.Activities
{
    public record ProcessAchievementPictureLog(
        string SmallDestinationKey,
        string MediumDestinationKey,
        string LargeDestinationKey);
}
