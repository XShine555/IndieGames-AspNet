using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetGameBuildsQuery(
        Guid UserId,
        Guid GameId)
        : IQuery<Result<IReadOnlyCollection<ApplicationGameBuild>>>;
}
