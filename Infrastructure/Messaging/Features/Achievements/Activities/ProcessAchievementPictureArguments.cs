using Application.Abstractions.Common;

namespace Infrastructure.Messaging.Features.Achievements.Activities
{
    public record ProcessAchievementPictureArguments(
        Guid AchievementId,
        string SourceKey,
        string SmallDestinationKey,
        string MediumDestinationKey,
        string LargeDestinationKey,
        PictureResizeSize SmallSize,
        PictureResizeSize MediumSize,
        PictureResizeSize LargeSize);
}
