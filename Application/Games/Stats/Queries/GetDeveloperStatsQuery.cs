using Application.Games.Stats.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Stats.Queries
{
    public record GetDeveloperStatsQuery(Guid DeveloperId)
        : IQuery<Result<DeveloperStatsResponse>>;
}
