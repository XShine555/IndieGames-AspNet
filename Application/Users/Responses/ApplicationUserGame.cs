using Application.Games.Responses;
using Application.Genres.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUserGame(
        Guid Id,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks);
}
