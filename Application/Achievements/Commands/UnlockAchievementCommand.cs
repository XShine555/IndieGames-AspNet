using Ardalis.Result;
using Mediator;

namespace Application.Achievements.Commands
{
    public record UnlockAchievementCommand(
        Guid UserId,
        Guid AchievementId)
        : ICommand<Result>;
}
