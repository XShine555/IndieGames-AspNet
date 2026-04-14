using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        int Id,
        string Title,
        string Description,
        decimal Price,
        decimal Discount,
        bool IsReadyForStore,
        bool IsPublic,
        bool IsPublished,
        ApplicationUserSummary ApplicationUserSummary,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures,
        IReadOnlyCollection<ApplicationGameArtwork> Artworks)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Price,
                game.Discount,
                game.StoreReadinessStatus == GameStoreReadinessStatus.ReadyForStore,
                game.IsPublic,
                game.IsPublished,
                ApplicationUserSummary.FromEntity(game.Owner),
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray(),
                game.StorePictures.Select(ApplicationGamePicture.FromEntity).ToArray(),
                game.Artworks.Select(ApplicationGameArtwork.FromEntity).ToArray());
        }
    }
}