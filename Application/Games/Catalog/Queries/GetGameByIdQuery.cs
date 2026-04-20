using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Catalog.Queries
{
    public record GetGameByIdQuery(Guid Id)
        : IQuery<Result<ApplicationGame>>;
}
