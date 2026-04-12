using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserPicture(
        int PictureId,
        string? OriginalName,
        string? OriginalFileExtension,
        string? OriginalRelativePath,
        string SmallRelativePath,
        string SmallName,
        string SmallFileExtension,
        string MediumRelativePath,
        string MediumName,
        string MediumFileExtension,
        string LargeRelativePath,
        string LargeName,
        string LargeFileExtension,
        DateTime AddedAt)
    {
        public static ApplicationUserPicture FromEntity(UserProfilePictures profilePicture)
        {
            return new ApplicationUserPicture(
                profilePicture.Id,
                profilePicture.OriginalName,
                profilePicture.OriginalFileExtension,
                profilePicture.OriginalRelativePath,
                profilePicture.SmallRelativePath,
                profilePicture.SmallName,
                profilePicture.SmallFileExtension,
                profilePicture.MediumRelativePath,
                profilePicture.MediumName,
                profilePicture.MediumFileExtension,
                profilePicture.LargeRelativePath,
                profilePicture.LargeName,
                profilePicture.LargeFileExtension,
                profilePicture.AddedAt);
        }
    }
}