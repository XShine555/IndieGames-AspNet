using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGameByIdQuery(long Id)
        : IQuery<Result<ApplicationGame>>;
}