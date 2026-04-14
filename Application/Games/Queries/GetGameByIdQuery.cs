using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGameByIdQuery(Guid Id)
        : IQuery<Result<ApplicationGame>>;
}