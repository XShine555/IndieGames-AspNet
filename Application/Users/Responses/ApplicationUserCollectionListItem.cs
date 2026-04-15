namespace Application.Users.Responses
{
    public record ApplicationUserCollectionListItem(
        Guid Id,
        string Name,
        int GamesCount,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
