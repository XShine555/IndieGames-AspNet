using Application.Games.Catalog.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUserCollectionDetails(
        Guid Id,
        string Name,
        IReadOnlyCollection<ApplicationGame> Games,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
