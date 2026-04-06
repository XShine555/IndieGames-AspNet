using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Genres.Queries
{
    public record GetGenreByIdQuery(Guid Id)
        : IQuery<Result<ApplicationGenre>>;
}