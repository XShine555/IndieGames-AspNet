namespace Application.Achievements.Responses
{
    public record ApplicationUserAchievement(
        Guid GameId,
        Guid AchievementId,
        string Name,
        string Description,
        string? SmallPicturePath,
        string? MediumPicturePath,
        string? LargePicturePath,
        bool IsUnlocked,
        DateTime? UnlockedAt);
}