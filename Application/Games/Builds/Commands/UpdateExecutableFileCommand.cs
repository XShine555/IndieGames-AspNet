using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record UpdateExecutableFileCommand(
        Guid BuildId,
        Guid UserId,
        string FileKey)
        : ICommand<Result<ApplicationGameBuildMutation>>;
}