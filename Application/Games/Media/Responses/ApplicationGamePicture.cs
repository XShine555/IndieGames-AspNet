using Domain.Entities;

namespace Application.Games.Media.Responses
{
    public record ApplicationGamePicture(
        Guid PictureId,
        string OriginalPictureKey,
        string? SmallPictureKey,
        string? MediumPictureKey,
        string? LargePictureKey,
        GamePictureProcessingStatus ProcessingStatus,
        DateTime AddedAt);
}
