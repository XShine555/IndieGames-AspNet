using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Commands
{
    public record PreSignGameFilesRequestCommand(
        Guid UserId,
        Guid BuildId,
        string[] FilePaths)
        : ICommand<Result<IReadOnlyList<ApplicationPreSignGameFileRequestMutation>>>;
}
