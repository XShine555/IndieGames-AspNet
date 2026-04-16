using Application.Abstractions.Common;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Users.Mappers
{
    public class UserMapper(IGameMapper gameMapper) : IUserMapper
    {
        public ApplicationUser ToApplicationUser(User user)
        {
            var createdGames = user.CreatedGames
                .Select(gameMapper.ToApplicationGame)
                .ToArray();

            var ownedGames = user.OwnedGames
                .Select(ownedGame => gameMapper.ToApplicationGame(ownedGame.Game))
                .ToArray();

            var cartItems = user.CartItems
                .Select(ToApplicationUserCartItem)
                .ToArray();

            return new ApplicationUser(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                ToApplicationUserPicture(user.ProfilePicture),
                createdGames,
                ownedGames,
                cartItems,
                user.CreatedAt,
                user.UpdatedAt);
        }

        public ApplicationUserPicture? ToApplicationUserPicture(UserProfilePictures? profilePicture)
        {
            if (profilePicture is null)
                return null;

            return new ApplicationUserPicture(
                profilePicture.Id,
                BuildKey(profilePicture.OriginalRelativePath, profilePicture.OriginalName),
                BuildKey(profilePicture.SmallRelativePath, profilePicture.SmallName),
                BuildKey(profilePicture.MediumRelativePath, profilePicture.MediumName),
                BuildKey(profilePicture.LargeRelativePath, profilePicture.LargeName),
                profilePicture.AddedAt);
        }

        public ApplicationUserMutation ToApplicationUserMutation(User user)
        {
            return new ApplicationUserMutation(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                user.UpdatedAt);
        }

        public ApplicationUserOwnedGame ToApplicationUserOwnedGame(UserOwnedGame relation)
        {
            return new ApplicationUserOwnedGame(
                relation.UserId,
                relation.GameId,
                relation.purchasedAt);
        }

        public ApplicationUserCartItem ToApplicationUserCartItem(UserCartItem cartItem)
        {
            return new ApplicationUserCartItem(
                cartItem.GameId,
                gameMapper.ToApplicationGame(cartItem.Game),
                cartItem.AddedAt);
        }

        public ApplicationUserCollectionListItem ToApplicationUserCollectionListItem(
            UserGameCollection collection,
            int gamesCount,
            string[]? previewSmallPictureUrls = null)
        {
            return new ApplicationUserCollectionListItem(
                collection.Id,
                collection.Name,
                gamesCount,
                previewSmallPictureUrls ?? Array.Empty<string>(),
                collection.CreatedAt,
                collection.UpdatedAt);
        }

        private static string BuildKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return $"{relativePath}/{name}";
        }
    }
}
