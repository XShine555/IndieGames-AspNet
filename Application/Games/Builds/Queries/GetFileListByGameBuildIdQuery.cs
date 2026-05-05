using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetFileListByGameBuildIdQuery(
        Guid BuildId,
        Guid UserId)
        : IQuery<Result<IReadOnlyList<string>> >;
}
