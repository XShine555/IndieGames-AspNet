using Application.Abstractions.Common;
using Application.Games.Responses;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGamesQuery(
        string Title,
        ICollection<Guid> Genres,
        int PageNumber,
        int PageSize,
        bool ReadyOnly = true)
        : IQuery<PaginatedApplicationResponse<ApplicationGame>>;
}