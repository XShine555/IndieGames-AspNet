using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        Guid Id,
        string Title,
        string Description,
        ICollection<ApplicationGenre> Genres)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray());
        }
    }
}