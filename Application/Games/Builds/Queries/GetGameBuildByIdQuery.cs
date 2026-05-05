using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetGameBuildByIdQuery(
        Guid UserId,
        Guid BuildId)
        : IQuery<Result<ApplicationGameBuild>>;
}
