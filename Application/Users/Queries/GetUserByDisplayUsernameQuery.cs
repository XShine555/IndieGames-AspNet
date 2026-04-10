using Application.Users.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUserByDisplayUsernameQuery(string DisplayUsername)
        : IQuery<Result<ApplicationUser>>;
}