using Application.Games.Builds.Responses;
using Ardalis.Result;
using Domain.Games.Entities;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record CreateGameBuildCommand(
        Guid UserId,
        Guid GameId,
        string VersionName)
        : ICommand<Result<ApplicationGameBuildMutation>>
    {
        public static GameBuild ToEntity(CreateGameBuildCommand command)
        {
            return new GameBuild
            {
                GameId = command.GameId,
                VersionName = command.VersionName,
            };
        }
    }
}
