namespace Application.Achievements.Responses
{
    public record ApplicationUserAchievement(
        Guid GameId,
        Guid AchievementId,
        string Name,
        string Description,
        bool IsUnlocked,
        DateTime? UnlockedAt);
}