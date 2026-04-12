using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserPicture(
        int PictureId,
        string? OriginalPictureKey,
        string SmallPictureKey,
        string MediumPictureKey,
        string LargePictureKey,
        DateTime AddedAt)
    {
        public static ApplicationUserPicture FromEntity(UserProfilePictures profilePicture)
        {
            return new ApplicationUserPicture(
                profilePicture.Id,
                profilePicture.OriginalRelativePath + profilePicture.OriginalName,
                profilePicture.SmallRelativePath + profilePicture.SmallName,
                profilePicture.MediumRelativePath + profilePicture.MediumName,
                profilePicture.LargeRelativePath + profilePicture.LargeName,
                profilePicture.AddedAt);
        }
    }
}