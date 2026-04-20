namespace Application.Games.Catalog.Responses
{
    public record ApplicationGameGenresMutation(
        Guid GameId,
        IReadOnlyCollection<Guid> GenreIds,
        DateTime UpdatedAt);
}
