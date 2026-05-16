using Application.Abstractions.Common;
using Application.Games.Catalog.Responses;
using Mediator;

namespace Application.Games.Catalog.Queries
{
    public record GetGamesQuery(
        string? Title,
        ICollection<Guid>? Genres,
        int PageNumber,
        int PageSize,
        GameCatalogQueryMode Mode = GameCatalogQueryMode.User)
        : IQuery<PaginatedApplicationResponse<ApplicationGameListItem>>;
}
