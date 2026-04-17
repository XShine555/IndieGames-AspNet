using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserCartItemsQuery(Guid UserId)
        : IQuery<Result<IReadOnlyCollection<ApplicationUserCartItem>> >;
}
