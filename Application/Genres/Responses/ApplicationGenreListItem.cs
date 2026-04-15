namespace Application.Genres.Responses
{
    public record ApplicationGenreListItem(
        Guid Id,
        string Name,
        int GamesCount,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
