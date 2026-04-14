using Application.Abstractions.Common;
using Application.Genres.Responses;
using Mediator;

namespace Application.Genres.Queries
{
    public record GetGenresQuery(
        string? Name = null,
        int PageNumber = 1,
        int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationGenre>>;
}