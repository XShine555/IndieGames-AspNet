namespace Application.Games.Responses
{
    public record ApplicationGameGenresMutation(
        Guid GameId,
        IReadOnlyCollection<Guid> GenreIds,
        DateTime UpdatedAt);
}
