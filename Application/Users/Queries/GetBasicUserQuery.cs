using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetBasicUserQuery(
        Guid UserId)
        : IQuery<Result<ApplicationBasicUser>>;
}