using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserByIdentityIdQuery(string IdentityId)
        : IQuery<Result<ApplicationUser>>;
}