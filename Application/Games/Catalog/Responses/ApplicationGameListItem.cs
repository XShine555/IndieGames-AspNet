using Application.Games.Media.Responses;
using Application.Genres.Responses;
using Application.Users.Responses;

namespace Application.Games.Catalog.Responses
{
    public record ApplicationGameListItem(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        bool IsPublic,
        bool IsPublished,
        ApplicationUserMutation Owner,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks,
        GameStatusType GameStatus,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
