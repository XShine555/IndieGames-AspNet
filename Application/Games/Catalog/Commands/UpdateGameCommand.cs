using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Commands
{
    public record UpdateGameCommand(
        Guid IdentityId,
        Guid GameId,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        bool IsPublic)
        : ICommand<Result<ApplicationGameMutation>>;
}
