using Application.Genres.Responses;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Games.Responses
{
    public record ApplicationGame(
        Guid Id,
        string Title,
        string Description,
        ICollection<ApplicationGenre> Genres,
        ICollection<ApplicationUser> Users,
        ICollection<ApplicationUser> RequestedUsers)
    {
        public static ApplicationGame FromEntity(Game game)
        {
            return new ApplicationGame(
                game.Id,
                game.Title,
                game.Description,
                game.Genres.Select(ApplicationGenre.FromEntity).ToArray(),
                game.Users.Select(ApplicationUser.FromEntity).ToArray(),
                game.RequestedUsers.Select(ApplicationUser.FromEntity).ToArray());
        }
    }
}