using Application.Games.Responses;
using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        string IdentityId,
        string Username,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames)
    {
        public static ApplicationUser FromEntity(User user, InfrastructureUser infrastructureUser)
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

            return new ApplicationUser(user.IdentityId, infrastructureUser.Username, createdGames, ownedGames);
        }
    }
}