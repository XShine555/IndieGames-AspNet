using Application.Games.Media.Responses;
using Application.Genres.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUserGame(
        Guid Id,
        string Title,
        string Description,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks);
}
