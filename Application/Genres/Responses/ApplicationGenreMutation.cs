namespace Application.Genres.Responses
{
    public record ApplicationGenreMutation(
        Guid Id,
        string Name,
        DateTime UpdatedAt);
}
