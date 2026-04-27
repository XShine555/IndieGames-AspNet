using Application.Abstractions.Common;
using Application.Configuration;
using Application.Games.Catalog.Responses;
using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;
using Domain.Games.Entities;

namespace Application.Games.Catalog.Mappers
{
    public class GameCatalogMapper(
        IGameMediaMapper gameMediaMapper,
        GameConfiguration gameConfiguration) : IGameCatalogMapper
    {
        public ApplicationGame ToApplicationGame(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Discount,
                game.IsPublic,
                game.IsPublished,
                ToOwnerMutation(game.Owner),
                game.Genres.Select(genre => new ApplicationGenre(genre.Id, genre.Name, genre.CreatedAt, genre.UpdatedAt)).ToArray(),
                game.StorePictures.Select(gameMediaMapper.ToApplicationGamePicture).ToArray(),
                game.Artworks.Select(gameMediaMapper.ToApplicationGameArtwork).ToArray(),
                ToReleaseBuild(game),
                game.CreatedAt,
                game.UpdatedAt);
        }

        public ApplicationCreatedGameListItem ToApplicationCreatedGameListItem(Game game)
        {
            var status = GameStatusType.NotPublished;
            if (game.IsPublished)
            {
                status= GameStatusType.Published;
            }
            if (game.Artworks.Any(x => x.ProcessingStatus == GameArtworkProcessingStatus.Failed)
                || game.StorePictures.Any(x => x.ProcessingStatus == GamePictureProcessingStatus.Failed))
            {
                status = GameStatusType.NotPublished;
            }

            var artworkPicture = game.Artworks.First(x => x.Type == GameArtworkType.Capsule);

            return new ApplicationCreatedGameListItem(
                game.Id,
                game.Title,
                game.Description,
                status,
                $"{artworkPicture.MediumRelativePath}/{artworkPicture.MediumFileName}");
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
                game.ReleaseGameBuildId,
                game.UpdatedAt);
        }

        public ApplicationGameGenresMutation ToApplicationGameGenresMutation(Game game)
        {
            return new ApplicationGameGenresMutation(
                game.Id,
                game.Genres.Select(g => g.Id).ToArray(),
                game.UpdatedAt);
        }

        private ApplicationGameReleaseBuild? ToReleaseBuild(Game game)
        {
            var releaseBuild = game.ReleaseGameBuild;
            if (releaseBuild is null)
                return null;

            return new ApplicationGameReleaseBuild(
                releaseBuild.Id,
                releaseBuild.VersionName,
                BuildManifestS3Path(game.Id, releaseBuild));
        }

        private string BuildManifestS3Path(Guid gameId, GameBuild releaseBuild)
        {
            if (string.IsNullOrWhiteSpace(releaseBuild.ManifestFileName))
                return string.Empty;

            return gameConfiguration.Routes.BuildGameBuildFilePath(gameId, releaseBuild.Id, releaseBuild.ManifestFileName);
        }

        private static ApplicationUserMutation ToOwnerMutation(User owner)
        {
            return new ApplicationUserMutation(
                owner.IdentityId,
                owner.Username,
                owner.DisplayUsername,
                owner.Role,
                owner.UpdatedAt);
        }
    }
}
