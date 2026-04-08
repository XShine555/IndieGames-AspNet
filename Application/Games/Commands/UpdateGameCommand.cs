using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameCommand(
        Guid GameId,
        string? Title,
        string? Description,
        string? OwnerId,
        ICollection<Guid> Genres)
        : ICommand<Result<ApplicationGame>>;
}