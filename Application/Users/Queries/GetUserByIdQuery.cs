using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserByIdQuery(Guid Id)
        : IQuery<Result<ApplicationUser>>;
}