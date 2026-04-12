using Application.Games.Responses;
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

    public record ApplicationUser(
        string IdentityId,
        string Username,
        string DisplayUsername,
        ApplicationUserPicture? ProfilePicture,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames,
        DateTime CreatedAt,
        DateTime UpdatedAt)
    {
        public static ApplicationUser FromEntity(User user)
        {
            var createdGames = new List<ApplicationGame>();
            foreach (var createdGame in user.CreatedGames)
            {
                createdGames.Add(ApplicationGame.FromEntity(createdGame));
            }

            var ownedGames = new List<ApplicationGame>();
            foreach (var ownedGame in user.OwnedGames)
            {
                ownedGames.Add(ApplicationGame.FromEntity(ownedGame.Game));
            }

            return new ApplicationUser(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                user.ProfilePicture is null ? null : ApplicationUserPicture.FromEntity(user.ProfilePicture),
                createdGames,
                ownedGames,
                user.CreatedAt,
                user.UpdatedAt);
        }
    }
}