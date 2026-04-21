using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Queries
{
    public record GetGameByIdQuery(
        Guid Id,
        GameCatalogQueryMode Mode = GameCatalogQueryMode.User)
        : IQuery<Result<ApplicationGame>>;
}
