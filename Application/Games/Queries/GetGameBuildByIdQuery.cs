using Application.Games.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Queries
{
    public record GetGameBuildByIdQuery(
        Guid UserId,
        Guid BuildId)
        : IQuery<Result<ApplicationGameBuild>>;
}
