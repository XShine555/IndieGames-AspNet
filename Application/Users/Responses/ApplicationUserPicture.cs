using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserPicture(
        Guid PictureId,
        string? OriginalPictureKey,
        string? SmallPictureKey,
        string? MediumPictureKey,
        string? LargePictureKey,
        DateTime AddedAt);
}