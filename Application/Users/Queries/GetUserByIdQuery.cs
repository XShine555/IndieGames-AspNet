using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserByIdQuery(long Id)
        : IQuery<Result<ApplicationUser>>;
}