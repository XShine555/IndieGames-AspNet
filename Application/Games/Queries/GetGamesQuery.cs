using Application.Contracts.Application;
using Application.Games.Responses;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGamesQuery(
        string Title,
        ICollection<int> Genres,
        int PageNumber,
        int PageSize)
        : IQuery<PaginatedApplicationResponse<ApplicationGame>>;
}