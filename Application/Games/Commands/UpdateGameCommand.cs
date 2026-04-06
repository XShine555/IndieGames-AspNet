using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Commands
{
    public record UpdateGameCommand(
        long Id,
        string? Title,
        string? Description)
        : ICommand<Result<ApplicationGame>>;
}