using Application.Abstractions.Common;
using Application.Users.Responses;
using Mediator;

namespace Application.Users.Queries
{
    public record GetUsersQuery(string DisplayUsername = "", int PageNumber = 1, int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationUser> >;
}