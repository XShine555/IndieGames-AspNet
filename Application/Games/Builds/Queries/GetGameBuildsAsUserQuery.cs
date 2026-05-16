using Application.Abstractions.Common;
using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetGameBuildsAsUserQuery(
        Guid UserId,
        Guid GameId,
        string? Title,
        int PageNumber,
        int PageSize)
        : IQuery<Result<PaginatedApplicationResponse<ApplicationGameBuildListItem> >>;
}
