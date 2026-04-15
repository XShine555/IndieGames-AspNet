using Application.Genres.Responses;
using Application.Users.Responses;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        bool IsReadyForStore,
        bool IsPublic,
        bool IsPublished,
        ApplicationUserMutation Owner,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}