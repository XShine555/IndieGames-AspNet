using Application.Abstractions.Common;

namespace Application.Abstractions.Messaging.Games.V1
{
    public record GenerateGameArtworksEvent(
        int ArtworkId,
        string SourceKey,
        string SmallDestinationRoute,
        string MediumDestinationRoute,
        string LargeDestinationRoute,
        PictureResizeSize SmallSize,
        PictureResizeSize MediumSize,
        PictureResizeSize LargeSize);
}
