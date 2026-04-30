using Application.Abstractions.Common;
using Application.Configuration;
using Application.Games.Catalog.Responses;
using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;
using Domain.Games.Entities;
using System.Linq.Expressions;

namespace Application.Games.Catalog.Mappers
{
    public class GameCatalogMapper(
        IGameMediaMapper gameMediaMapper,
        IGenreMapper genreMapper,
        GameConfiguration gameConfiguration) : IGameCatalogMapper
    {
        public Expression<Func<Game, ApplicationGameListItem>> ToApplicationGameListItemFunction => g => new ApplicationGameListItem(
            g.Id,
            g.Title,
            g.Description,
            g.Price,
            g.Discount,
            g.IsPublic,
            g.IsPublished,
            new ApplicationUserMutation(
                g.Owner.IdentityId,
                g.Owner.Username,
                g.Owner.DisplayUsername,
                g.Owner.Role,
                g.Owner.UpdatedAt),
            g.Genres.AsQueryable().Select(genreMapper.ToApplicationGenreFunction).ToList(),
            g.Artworks.AsQueryable().Select(gameMediaMapper.ToApplicationGameArtworkFunction).ToList(),
            g.Artworks.Any(x => x.ProcessingStatus == GameArtworkProcessingStatus.Failed)
                || g.StorePictures.Any(x => x.ProcessingStatus == GamePictureProcessingStatus.Failed)
                ? GameStatusType.WithErrors
                : g.IsPublished
                    ? GameStatusType.Published
                    : GameStatusType.NotPublished,
            g.CreatedAt,
            g.UpdatedAt);

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
                GetGameStatus(game),
                game.CreatedAt,
                game.UpdatedAt);
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

        ApplicationGameReleaseBuild? ToReleaseBuild(Game game)
        {
            var releaseBuild = game.ReleaseGameBuild;
            if (releaseBuild is null)
                return null;

            return new ApplicationGameReleaseBuild(
                releaseBuild.Id,
                releaseBuild.VersionName,
                BuildManifestS3Path(game.Id, releaseBuild));
        }

        string BuildManifestS3Path(Guid gameId, GameBuild releaseBuild)
        {
            if (string.IsNullOrWhiteSpace(releaseBuild.ManifestFileName))
                return string.Empty;

            return gameConfiguration.Routes.BuildGameBuildFilePath(gameId, releaseBuild.Id, releaseBuild.ManifestFileName);
        }

        static ApplicationUserMutation ToOwnerMutation(User owner)
        {
            return new ApplicationUserMutation(
                owner.IdentityId,
                owner.Username,
                owner.DisplayUsername,
                owner.Role,
                owner.UpdatedAt);
        }

        static GameStatusType GetGameStatus(Game game)
        {
            if (game.Artworks.Any(x => x.ProcessingStatus == GameArtworkProcessingStatus.Failed)
                || game.StorePictures.Any(x => x.ProcessingStatus == GamePictureProcessingStatus.Failed))
                return GameStatusType.WithErrors;

            if (game.IsPublished)
                return GameStatusType.Published;

            return GameStatusType.NotPublished;
        }
    }
}
