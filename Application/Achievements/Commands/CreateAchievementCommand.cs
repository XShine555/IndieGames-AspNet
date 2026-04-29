using Application.Abstractions.Common;
using Application.Achievements.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Achievements.Commands
{
    public record CreateAchievementCommand(
        Guid GameId,
        Guid UserId,
        string Name,
        string Description,
        IFileData Picture)
        : ICommand<Result<ApplicationAchievement>>;
}
