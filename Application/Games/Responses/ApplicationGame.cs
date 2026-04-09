using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        int Id,
        string Title,
        string Description,
        ApplicationUserSummary ApplicationUserSummary,
        IReadOnlyCollection<ApplicationGenre> Genres,
        IReadOnlyCollection<ApplicationGamePicture> Pictures)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                ApplicationUserSummary.FromEntity(game.Owner),
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray(),
                game.Pictures.Select(ApplicationGamePicture.FromEntity).ToArray());
        }
    }
}