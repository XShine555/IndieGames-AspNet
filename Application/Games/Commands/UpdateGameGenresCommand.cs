using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameGenresCommand(
        string IdentityId,
        int GameId,
        ICollection<int> Genres)
        : ICommand<Result<ApplicationGame>>;
}
