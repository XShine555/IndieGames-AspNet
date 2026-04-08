using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Genres.Queries
{
    public record GetGenreByIdQuery(int Id)
        : IQuery<Result<ApplicationGenre>>;
}