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
            {
                string executableS3Path = BuildStoragePath(gameBuild.ExecutableRelativePath, gameBuild.ExecutableFileName);
                var buildIdString = gameBuild.Id.ToString();
                var index = executableS3Path.IndexOf(buildIdString);
                if (index != -1)
                {
                    executablePath = executableS3Path.Substring(index + buildIdString.Length + 1);
                }
            }

            return new ApplicationGameBuildMutation(
                gameBuild.Id,
                gameBuild.VersionName,
                executablePath,
                gameBuild.CreatedAt);
        }

        public ApplicationGameBuild ToApplicationGameBuild(GameBuild gameBuild, bool isReleaseBuild)
        {
            string? manifestStoragePath = null;
            if (!string.IsNullOrEmpty(gameBuild.ManifestRelativePath) && !string.IsNullOrEmpty(gameBuild.ManifestFileName))
                manifestStoragePath = BuildStoragePath(gameBuild.ManifestRelativePath, gameBuild.ManifestFileName);

            string? pathAfterBuild = null;
            if (!string.IsNullOrEmpty(gameBuild.ExecutableRelativePath) && !string.IsNullOrEmpty(gameBuild.ExecutableFileName))
            {
                string executableS3Path = BuildStoragePath(gameBuild.ExecutableRelativePath, gameBuild.ExecutableFileName);
                var buildIdString = gameBuild.Id.ToString();
                var index = executableS3Path.IndexOf(buildIdString);

                if (index == -1)
                    throw new InvalidOperationException(
                        $"Build ID '{buildIdString}' not found in S3 path '{executableS3Path}'.");

                var startIndex = index + buildIdString.Length;
                pathAfterBuild = startIndex < executableS3Path.Length
                    ? executableS3Path.Substring(startIndex + 1)
                    : string.Empty;
            }

            return new ApplicationGameBuild(
                gameBuild.Id,
                gameBuild.VersionName,
                manifestStoragePath,
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

        public ApplicationFileInfo ToApplicationFileInfo(GameBuildFile gameBuildFile)
        {
            return new ApplicationFileInfo(
                gameBuildFile.Id,
                gameBuildFile.GameBuildId,
                BuildStoragePath(gameBuildFile.FileRelativePath, gameBuildFile.FileName),
                gameBuildFile.FileSize,
                gameBuildFile.Hash,
                gameBuildFile.HashAlgorithm);
        }

        string BuildStoragePath(params string[] values)
        {
            return string.Join("/", values.Select(v => v.Replace('\\', '/')));
        }
    }
}
