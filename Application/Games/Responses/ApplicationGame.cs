using Application.Genres.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        int Id,
        string Title,
        string Description,
        string OwnerId,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.OwnerId,
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray(),
                game.Pictures.Select(ApplicationGamePicture.FromEntity).ToArray());
        }
    }
}