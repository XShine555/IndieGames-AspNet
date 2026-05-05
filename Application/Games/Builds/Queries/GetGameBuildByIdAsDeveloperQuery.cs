using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetGameBuildByIdAsDeveloperQuery(
        Guid BuildId,
        Guid UserId)
        : IQuery<Result<ApplicationGameBuild>>;
}