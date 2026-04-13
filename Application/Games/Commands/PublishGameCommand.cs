using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record PublishGameCommand(
        string IdentityId,
        int GameId)
        : ICommand<Result<ApplicationGame>>;
}
