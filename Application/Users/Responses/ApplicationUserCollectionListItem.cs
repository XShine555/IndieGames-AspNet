namespace Application.Users.Responses
{
    public record ApplicationUserCollectionListItem(
        Guid Id,
        string Name,
        int GamesCount,
        string[] PreviewSmallPictureUrls,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
