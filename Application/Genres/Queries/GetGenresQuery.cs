using Application.Abstractions;
using Application.Genres.Responses;
using Mediator;

namespace Application.Genres.Queries
{
    public record GetGenresQuery(
        string Name,
        int PageNumber,
        int PageSize)
        : IQuery<PaginatedApplicationResponse<ApplicationGenre>>;
}