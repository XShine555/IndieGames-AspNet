using Application.Abstractions.Common;
using Application.Games.Catalog.Responses;
using Mediator;

namespace Application.Games.Catalog.Queries
{
    public record GetGamesByOwnerIdQuery(
        Guid UserId,
        int PageNumber,
        int PageSize)
        : IQuery<PaginatedApplicationResponse<ApplicationGameListItem>>;
}