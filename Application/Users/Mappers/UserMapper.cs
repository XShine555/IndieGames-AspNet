using Application.Abstractions.Common;
using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;
using System.Linq.Expressions;

namespace Application.Users.Mappers
{
    public class UserMapper(IGameMediaMapper gameMediaMapper) : IUserMapper
    {
        public ApplicationUser ToApplicationUser(User user)
        {
            var createdGames = user.CreatedGames
                .Select(ToApplicationUserGame)
                .ToArray();

            var ownedGames = user.OwnedGames
                .Select(ownedGame => ToApplicationUserGame(ownedGame.Game))
                .ToArray();

            var cartItems = user.CartItems
                .Select(ToApplicationUserCartItem)
                .ToArray();

            return new ApplicationUser(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                user.Role,
                ToApplicationUserPicture(user.ProfilePicture),
                createdGames,
                ownedGames,
                cartItems,
                user.CreatedAt,
                user.UpdatedAt);
        }

        public ApplicationUserPicture ToApplicationUserPicture(UserProfilePictures profilePicture)
        {
            return new ApplicationUserPicture(
                    profilePicture.Id,
                    BuildKey(profilePicture.OriginalRelativePath, profilePicture.OriginalName),
                    BuildKey(profilePicture.SmallRelativePath, profilePicture.SmallName),
                    BuildKey(profilePicture.MediumRelativePath, profilePicture.MediumName),
                    BuildKey(profilePicture.LargeRelativePath, profilePicture.LargeName),
                    profilePicture.AddedAt);
        }

        public Expression<Func<UserProfilePictures, ApplicationUserPicture>> ToApplicationUserPictureExpression()
        {
            return profilePicture => new ApplicationUserPicture(
                profilePicture.Id,
                (string.IsNullOrWhiteSpace(profilePicture.OriginalRelativePath) || string.IsNullOrWhiteSpace(profilePicture.OriginalName))
                    ? string.Empty
                    : $"{profilePicture.OriginalRelativePath}/{profilePicture.OriginalName}",
                (string.IsNullOrWhiteSpace(profilePicture.SmallRelativePath) || string.IsNullOrWhiteSpace(profilePicture.SmallName))
                    ? string.Empty
                    : $"{profilePicture.SmallRelativePath}/{profilePicture.SmallName}",
                (string.IsNullOrWhiteSpace(profilePicture.MediumRelativePath) || string.IsNullOrWhiteSpace(profilePicture.MediumName))
                    ? string.Empty
                    : $"{profilePicture.MediumRelativePath}/{profilePicture.MediumName}",
                (string.IsNullOrWhiteSpace(profilePicture.LargeRelativePath) || string.IsNullOrWhiteSpace(profilePicture.LargeName))
                    ? string.Empty
                    : $"{profilePicture.LargeRelativePath}/{profilePicture.LargeName}",
                profilePicture.AddedAt);
        }

        public Expression<Func<User, ApplicationUserListItem>> ToApplicationUserListItemExpression()
        {
            var originalRelativeKey = string.Empty;
            if (!string.IsNullOrWhiteSpace(originalRelativeKey))
                originalRelativeKey = "{ProfilePicture.OriginalRelativePath}/{ProfilePicture.OriginalName}";

            return user => new ApplicationUserListItem(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                new ApplicationUserPicture(
                    user.ProfilePicture.Id,
                    originalRelativeKey,
                    $"{user.ProfilePicture.SmallRelativePath}/{user.ProfilePicture.SmallName}",
                    $"{user.ProfilePicture.MediumRelativePath}/{user.ProfilePicture.MediumName}",
                    $"{user.ProfilePicture.LargeRelativePath}/{user.ProfilePicture.LargeName}",
                    user.ProfilePicture.AddedAt),
                user.CreatedGames.Count,
                user.OwnedGames.Count,
                user.CreatedAt,
                user.UpdatedAt);
        }

        public ApplicationUserMutation ToApplicationUserMutation(User user)
        {
            return new ApplicationUserMutation(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                user.Role,
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
                ToApplicationUserGame(cartItem.Game),
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

        private ApplicationUserGame ToApplicationUserGame(Game game)
        {
            return new ApplicationUserGame(
                game.Id,
                game.Title,
                game.Description,
                game.Genres.Select(genre => new ApplicationGenre(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt)).ToArray(),
                game.StorePictures.Select(gameMediaMapper.ToApplicationGamePicture).ToArray(),
                game.Artworks.Select(gameMediaMapper.ToApplicationGameArtwork).ToArray());
        }

        private static string BuildKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return $"{relativePath}/{name}";
        }
    }
}
