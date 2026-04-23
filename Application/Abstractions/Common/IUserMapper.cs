using Application.Users.Responses;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Abstractions.Common
{
    public interface IUserMapper
    {
        ApplicationUser ToApplicationUser(User user);

        ApplicationBasicUser ToApplicationBasicUser(User user);

        ApplicationUserPicture ToApplicationUserPicture(UserProfilePictures profilePicture);

        Expression<Func<UserProfilePictures, ApplicationUserPicture>> ToApplicationUserPictureExpression();

        Expression<Func<User, ApplicationUserListItem>> ToApplicationUserListItemExpression();

        ApplicationUserMutation ToApplicationUserMutation(User user);

        ApplicationUserOwnedGame ToApplicationUserOwnedGame(UserOwnedGame relation);

        ApplicationUserCartItem ToApplicationUserCartItem(UserCartItem cartItem);

        ApplicationUserCollectionListItem ToApplicationUserCollectionListItem(
            UserGameCollection collection,
            int gamesCount,
            string[]? previewSmallPictureKeys);
    }
}
