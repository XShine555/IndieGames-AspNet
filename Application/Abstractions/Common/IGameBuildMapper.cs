using Application.Games.Builds.Responses;
using Domain.Games.Entities;

namespace Application.Abstractions.Common
{
    public interface IGameBuildMapper
    {
        ApplicationGameBuildMutation ToApplicationGameBuildMutation(GameBuild gameBuild);

        ApplicationGameBuild ToApplicationGameBuild(GameBuild gameBuild, bool isReleaseBuild);

        ApplicationGameBuildListItem ToApplicationGameBuildListItem(GameBuild gameBuild, bool isReleaseBuild);

        public ApplicationFileInfo ToApplicationFileInfo(GameBuildFile gameBuildFile);
    }
}
