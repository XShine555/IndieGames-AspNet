using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record UpdateGameBuildCommand(
        Guid UserId,
        Guid BuildId,
        string VersionName)
        : ICommand<Result<ApplicationGameBuildMutation>>;
}
