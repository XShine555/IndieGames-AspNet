using Ardalis.Result;
using Mediator;

namespace Application.Achievements.Commands
{
    public record PublishAchievementCommand(
        Guid UserId,
        Guid AchievementId)
        : ICommand<Result>;
}
