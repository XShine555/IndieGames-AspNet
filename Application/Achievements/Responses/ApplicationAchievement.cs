namespace Application.Achievements.Responses
{
    public record ApplicationAchievement(
        Guid Id,
        Guid GameId,
        string Name,
        string Description,
        bool IsUnlocked,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
