using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record ChangeGameOwnerCommand(
        string IdentityId,
        int GameId,
        string NewOwnerId)
        : ICommand<Result<ApplicationGame>>;
}
