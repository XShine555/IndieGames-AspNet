using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record AddGameGenresCommand(Guid GameId, ICollection<Guid> Genres)
        : ICommand<Result<ApplicationGame>>;
}
