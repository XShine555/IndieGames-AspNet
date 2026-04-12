using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserPicture(
        int PictureId,
        string? OriginalPictureKey,
        string? SmallPictureKey,
        string? MediumPictureKey,
        string? LargePictureKey,
        DateTime AddedAt)
    {
        public static ApplicationUserPicture FromEntity(UserProfilePictures profilePicture)
        {
            return new ApplicationUserPicture(
                profilePicture.Id,
                BuildPictureKey(profilePicture.OriginalRelativePath, profilePicture.OriginalName),
                BuildPictureKey(profilePicture.SmallRelativePath, profilePicture.SmallName),
                BuildPictureKey(profilePicture.MediumRelativePath, profilePicture.MediumName),
                BuildPictureKey(profilePicture.LargeRelativePath, profilePicture.LargeName),
                profilePicture.AddedAt);
        }

        static string BuildPictureKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;
            return $"{relativePath}/{name}";
        }
    }
}