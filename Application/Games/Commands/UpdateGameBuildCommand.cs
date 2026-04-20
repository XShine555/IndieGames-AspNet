using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameBuildCommand(
        Guid UserId,
        Guid BuildId,
        string VersionName)
        : ICommand<Result<ApplicationGameBuildMutation>>;
}
