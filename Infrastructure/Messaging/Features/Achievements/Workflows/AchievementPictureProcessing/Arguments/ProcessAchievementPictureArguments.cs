using Application.Abstractions.Common;

namespace Infrastructure.Messaging.Features.Achievements.Workflows.AchievementPictureProcessing.Arguments
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
