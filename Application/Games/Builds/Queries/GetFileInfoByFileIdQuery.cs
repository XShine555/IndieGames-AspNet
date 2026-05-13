using Application.Games.Builds.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Builds.Queries
{
    public record GetFileInfoByFileIdQuery(Guid FileId, Guid UserId)
        : IQuery<Result<ApplicationFileInfo>>;
}
