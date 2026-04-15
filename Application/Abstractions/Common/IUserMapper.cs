using Application.Users.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IUserMapper
    {
        ApplicationUser ToApplicationUser(User user);

        ApplicationUserPicture? ToApplicationUserPicture(UserProfilePictures? profilePicture);

        ApplicationUserMutation ToApplicationUserMutation(User user);

        ApplicationUserOwnedGame ToApplicationUserOwnedGame(UserOwnedGame relation);

        ApplicationUserCollectionListItem ToApplicationUserCollectionListItem(
            UserGameCollection collection,
            int gamesCount,
            string[]? previewSmallPictureKeys);
    }
}
