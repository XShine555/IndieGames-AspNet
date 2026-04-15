using Application.Abstractions.Common;
using Application.Users.Responses;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserCollectionsQuery(
        Guid UserId,
        int PageNumber = 1,
        int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationUserCollectionListItem>>;
}
