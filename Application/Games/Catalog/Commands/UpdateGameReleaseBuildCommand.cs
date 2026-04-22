using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Commands
{
    public record UpdateGameReleaseBuildCommand(
        Guid UserId,
        Guid GameId,
        Guid BuildId)
        : ICommand<Result<ApplicationGameMutation>>;
}