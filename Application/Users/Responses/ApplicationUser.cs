using Application.Games.Responses;
using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        string IdentityId,
        string Username,
        string DisplayUsername,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames,
        DateTime CreatedAt,
        DateTime UpdatedAt)
    {
        public static ApplicationUser FromEntity(User user)
        {
            var createdGames = new List<ApplicationGame>();
            foreach (var createdGame in user.CreatedGames)
            {
                createdGames.Add(ApplicationGame.FromEntity(createdGame));
            }

            var ownedGames = new List<ApplicationGame>();
            foreach (var ownedGame in user.OwnedGames)
            {
                ownedGames.Add(ApplicationGame.FromEntity(ownedGame.Game));
            }

            return new ApplicationUser(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                createdGames,
                ownedGames,
                user.CreatedAt,
                user.UpdatedAt);
        }
    }
}