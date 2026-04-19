using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record PreSignGameFilesRequestCommand(
        Guid UserId,
        Guid BuildId,
        string[] FilePaths)
        : ICommand<Result<IReadOnlyList<ApplicationPreSignGameFileRequestMutation> >>;
}