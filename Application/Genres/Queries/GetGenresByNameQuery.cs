using Application.Contracts.Application;
using Application.Genres.Responses;
using Mediator;

namespace Application.Genres.Queries
{
    public record GetGenresByNameQuery(
        string Name,
        int PageNumber = 1,
        int PageSize = 10)
        : IQuery<PaginatedApplicationResponse<ApplicationGenre>>;
}