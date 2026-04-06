using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddGameRequestedUsersCommand(Guid GameId, ICollection<Guid> RequestedUsers)
        : ICommand<Result<ApplicationGame>>;
}