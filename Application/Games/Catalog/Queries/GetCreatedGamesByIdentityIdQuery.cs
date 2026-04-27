using Application.Abstractions.Common;
using Application.Games.Catalog.Responses;
using Mediator;

namespace Application.Games.Catalog.Queries
{
    public record GetCreatedGamesByIdentityIdQuery(
        Guid UserId,
        string Title,
        int PageNumber,
        int PageSize)
        : IQuery<PaginatedApplicationResponse<ApplicationCreatedGameListItem>>;
}
