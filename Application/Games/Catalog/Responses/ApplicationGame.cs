using Application.Games.Media.Responses;
using Application.Genres.Responses;
using Application.Users.Responses;

namespace Application.Games.Catalog.Responses
{
    public record ApplicationGame(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        bool IsPublic,
        bool IsPublished,
        ApplicationUserMutation Owner,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks,
        ApplicationGameReleaseBuild? ReleaseBuild,
        GameStatusType GameStatus,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
