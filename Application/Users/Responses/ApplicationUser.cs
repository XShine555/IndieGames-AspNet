using Application.Games.Responses;
using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        string IdentityId,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames)
    {
        public static ApplicationUser FromEntity(User user)
        {
            return new ApplicationUser(
                user.IdentityId,
                user.CreatedGames.Select(ApplicationGame.FromEntity).ToList(),
                user.OwnedGames.Select(ownedGame => ApplicationGame.FromEntity(ownedGame.Game)).ToList());
        }
    }
}