using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserCollectionByIdQuery(
        Guid UserId,
        Guid CollectionId)
        : IQuery<Result<ApplicationUserCollectionDetails>>;
}
