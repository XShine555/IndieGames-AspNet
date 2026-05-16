using Domain.Games.Enums;

namespace Application.Achievements.Responses
{
    public record ApplicationAchievement(
        Guid Id,
        Guid GameId,
        string Name,
        string Description,
        string? SmallPicturePath,
        string? MediumPicturePath,
        string? LargePicturePath,
        bool IsPublished,
        AchievementPictureProcessingStatus Status,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
