using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameGenresCommand(
        Guid IdentityId,
        Guid GameId,
        ICollection<Guid> Genres)
        : ICommand<Result<ApplicationGame>>;
}
