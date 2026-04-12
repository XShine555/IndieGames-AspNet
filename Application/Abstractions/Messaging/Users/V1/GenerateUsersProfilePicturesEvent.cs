using Application.Abstractions.Common;

namespace Application.Abstractions.Messaging.Users.V1
{
    public record GenerateUsersProfilePicturesEvent(
        int PictureId,
        string SourceKey,
        string SmallDestinationRoute,
        string MediumDestinationRoute,
        string LargeDestinationRoute,
        PictureResizeSize SmallSize,
        PictureResizeSize MediumSize,
        PictureResizeSize LargeSize);
}