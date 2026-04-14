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
        decimal? Price,
        decimal? Discount,
        bool? IsPublic)
        : ICommand<Result<ApplicationGame>>;
}