using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGameBuildsQuery(
        Guid UserId,
        Guid GameId)
        : IQuery<Result<IReadOnlyCollection<ApplicationGameBuild> >>;
}
