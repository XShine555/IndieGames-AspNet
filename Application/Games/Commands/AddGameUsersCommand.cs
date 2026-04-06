using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddGameUsersCommand(Guid GameId, ICollection<Guid> Users)
        : ICommand<Result<ApplicationGame>>;
}