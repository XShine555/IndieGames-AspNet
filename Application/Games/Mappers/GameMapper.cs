using Application.Abstractions.Common;
using Application.Genres.Responses;
using Application.Games.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Mappers
{
    public class GameMapper : IGameMapper
    {
        public ApplicationGame ToApplicationGame(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Discount,
                game.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore,
                game.IsPublic,
                game.IsPublished,
                ToOwnerMutation(game.Owner),
                game.Genres.Select(genre => new ApplicationGenre(genre.Id, genre.Name)).ToArray(),
                game.StorePictures.Select(ToApplicationGamePicture).ToArray(),
                game.Artworks.Select(ToApplicationGameArtwork).ToArray());
        }

        public ApplicationGamePicture ToApplicationGamePicture(GameStorePictures gamePicture)
        {
            return new ApplicationGamePicture(
                gamePicture.Id,
                BuildKey(gamePicture.OriginalRelativePath, gamePicture.OriginalName),
                BuildKey(gamePicture.SmallRelativePath, gamePicture.SmallName),
                BuildKey(gamePicture.MediumRelativePath, gamePicture.MediumName),
                BuildKey(gamePicture.LargeRelativePath, gamePicture.LargeName),
                gamePicture.ProcessingStatus,
                gamePicture.AddedAt);
        }

        public ApplicationGameArtwork ToApplicationGameArtwork(GameArtwork artwork)
        {
            return new ApplicationGameArtwork(
                artwork.Id,
                artwork.Type,
                BuildKey(artwork.OriginalRelativePath, artwork.OriginalFileName),
                BuildKey(artwork.SmallRelativePath, artwork.SmallFileName),
                BuildKey(artwork.MediumRelativePath, artwork.MediumFileName),
                BuildKey(artwork.LargeRelativePath, artwork.LargeFileName),
                artwork.ProcessingStatus,
                artwork.CreatedAt);
        }

        public ApplicationGameMutation ToApplicationGameMutation(Game game)
        {
            return new ApplicationGameMutation(
                game.Id,
                game.OwnerId,
                game.Title,
                game.Price,
                game.Discount,
                game.IsPublic,
                game.IsPublished,
                game.UpdatedAt);
        }

        public ApplicationGameGenresMutation ToApplicationGameGenresMutation(Game game)
        {
            return new ApplicationGameGenresMutation(
                game.Id,
                game.Genres.Select(g => g.Id).ToArray(),
                game.UpdatedAt);
        }

        private static ApplicationUserMutation ToOwnerMutation(User owner)
        {
            return new ApplicationUserMutation(
                owner.IdentityId,
                owner.Username,
                owner.DisplayUsername,
                owner.UpdatedAt);
        }

        private static string BuildKey(string? relativePath, string? name)
        {
            if (string.IsNullOrWhiteSpace(relativePath) || string.IsNullOrWhiteSpace(name))
                return string.Empty;

            return $"{relativePath}/{name}";
        }
    }
}
