using Application.Abstractions.Common;
using Application.Games.Builds.Responses;
using Domain.Games.Entities;

namespace Application.Games.Builds.Mappers
{
    public class GameBuildMapper : IGameBuildMapper
    {
        public ApplicationGameBuildMutation ToApplicationGameBuildMutation(GameBuild gameBuild)
        {
            string executablePath = null;
            if (!string.IsNullOrEmpty(gameBuild.ExecutableRelativePath) && !string.IsNullOrEmpty(gameBuild.ExecutableFileName))
                executablePath = BuildStoragePath(gameBuild.ExecutableRelativePath, gameBuild.ExecutableFileName);

            return new ApplicationGameBuildMutation(
                gameBuild.Id,
                gameBuild.VersionName,
                executablePath,
                gameBuild.CreatedAt);
        }

        public ApplicationGameBuild ToApplicationGameBuild(GameBuild gameBuild, bool isReleaseBuild)
        {
            string executableS3Path = BuildStoragePath(gameBuild.ExecutableRelativePath!, gameBuild.ExecutableFileName!);
            var buildIdString = gameBuild.Id.ToString();
            var index = executableS3Path.IndexOf(buildIdString);
            var pathAfterBuild = executableS3Path.Substring(index + buildIdString.Length + 1);

            return new ApplicationGameBuild(
                gameBuild.Id,
                gameBuild.VersionName,
                BuildStoragePath(gameBuild.ManifestRelativePath!, gameBuild.ManifestFileName!),
                pathAfterBuild,
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
