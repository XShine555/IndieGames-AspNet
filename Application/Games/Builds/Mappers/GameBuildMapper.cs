using Application.Abstractions.Common;
using Application.Games.Builds.Responses;
using Domain.Games.Entities;

namespace Application.Games.Builds.Mappers
{
    public class GameBuildMapper : IGameBuildMapper
    {
        public ApplicationGameBuildMutation ToApplicationGameBuildMutation(GameBuild gameBuild)
        {
            return new ApplicationGameBuildMutation(gameBuild.Id, gameBuild.VersionName, gameBuild.CreatedAt);
        }

        public ApplicationGameBuild ToApplicationGameBuild(GameBuild gameBuild, bool isReleaseBuild)
        {
            return new ApplicationGameBuild(
                gameBuild.Id,
                gameBuild.VersionName,
                BuildStoragePath(gameBuild.ManifestRelativePath, gameBuild.ManifestFileName),
                isReleaseBuild,
                gameBuild.Status,
                gameBuild.CreatedAt);
        }

        public ApplicationGameBuildListItem ToApplicationGameBuildListItem(GameBuild gameBuild, bool isReleaseBuild)
        {
            return new ApplicationGameBuildListItem(
                gameBuild.Id,
                gameBuild.VersionName,
                gameBuild.Status,
                isReleaseBuild);
        }

        string BuildStoragePath(params string[] values)
        {
            return string.Join("/", values);
        }
    }
}
