using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameCommand(
        string IdentityId,
        int GameId,
        string? Title,
        string? Description,
        string? OwnerId,
        ICollection<int> Genres)
        : ICommand<Result<ApplicationGame>>;
}