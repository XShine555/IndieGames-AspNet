using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record DeleteGameUsersCommand(Guid GameId, ICollection<Guid> Users)
        : ICommand<Result<ApplicationGame>>;
}