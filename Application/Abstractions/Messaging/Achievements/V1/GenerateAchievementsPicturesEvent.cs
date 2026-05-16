using Application.Abstractions.Common;

namespace Application.Abstractions.Messaging.Achievements.V1
{
    public record GenerateAchievementsPicturesEvent(
        Guid AchievementId,
        string SourceKey,
        string SmallDestinationKey,
        string MediumDestinationKey,
        string LargeDestinationKey,
        PictureResizeSize SmallSize,
        PictureResizeSize MediumSize,
        PictureResizeSize LargeSize);
}
