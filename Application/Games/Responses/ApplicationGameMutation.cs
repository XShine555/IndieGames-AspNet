namespace Application.Games.Responses
{
    public record ApplicationGameMutation(
        Guid Id,
        Guid OwnerId,
        string Title,
        decimal Price,
        decimal Discount,
        bool IsPublic,
        bool IsPublished,
        DateTime UpdatedAt);
}
