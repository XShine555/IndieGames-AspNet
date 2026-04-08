using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGameByIdQuery(int Id)
        : IQuery<Result<ApplicationGame>>;
}