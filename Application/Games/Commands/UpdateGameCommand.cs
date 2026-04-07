using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameCommand(
        Guid Id,
        string? Title,
        string? Description)
        : ICommand<Result<ApplicationGame>>;
}