using Application.Genres.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        Guid Id,
        string Title,
        string Description,
        string OwnerId,
        ICollection<ApplicationGenre> Genres)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.OwnerId,
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray());
        }
    }
}