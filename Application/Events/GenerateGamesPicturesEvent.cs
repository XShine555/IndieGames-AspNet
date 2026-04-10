namespace Application.Events
{
    public record GenerateGamesPicturesEvent(
        int PictureId,
        string SourceKey,
        string SmallDestinationRoute,
        string MediumDestinationRoute,
        string LargeDestinationRoute,
        PictureResizeSize SmallSize,
        PictureResizeSize MediumSize,
        PictureResizeSize LargeSize);

    public record PictureResizeSize(
        int Width,
        int Height);
}