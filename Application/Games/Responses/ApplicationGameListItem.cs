using Application.Users.Responses;

namespace Application.Games.Responses
{
    public record ApplicationGameListItem(
        Guid Id,
        string Title,
        decimal Price,
        decimal Discount,
        bool IsReadyForStore,
        bool IsPublic,
        bool IsPublished,
        ApplicationUserSummary Owner,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
