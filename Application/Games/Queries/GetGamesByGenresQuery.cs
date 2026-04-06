using Application.Contracts.Application;
using Application.Games.Responses;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGamesByGenresQuery(
        ICollection<Guid> GenresIds,
        int PageNumber = 1,
        int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationGame>>;
}