using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record PublishGameCommand(
        Guid IdentityId,
        Guid GameId)
        : ICommand<Result<ApplicationGame>>;
}
