using Ardalis.Result;
using Mediator;

namespace Application.Achievements.Commands
{
    public record RemoveAchievementCommand(
        Guid UserId,
        Guid AchievementId)
        : ICommand<Result>;
}
