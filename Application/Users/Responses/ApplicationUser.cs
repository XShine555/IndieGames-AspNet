using Application.Contracts.Application;
using Application.Games.Helpers;
using Application.Games.Responses;
using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        string IdentityId,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames)
    {
        public static async Task<ApplicationUser> FromEntity(User user, IGamePicturesHelper gamePicturesHelper,
            CancellationToken cancellationToken)
        {
            var createdGames = new List<ApplicationGame>();
            foreach (var createdGame in user.CreatedGames)
            {
                var pictures = await gamePicturesHelper.GetPictures(createdGame, cancellationToken);
                createdGames.Add(ApplicationGame.FromEntity(createdGame, pictures));
            }

            var ownedGames = new List<ApplicationGame>();
            foreach (var ownedGame in user.OwnedGames)
            {
                var pictures = await gamePicturesHelper.GetPictures(ownedGame.Game, cancellationToken);
                ownedGames.Add(ApplicationGame.FromEntity(ownedGame.Game, pictures));
            }

            return new ApplicationUser(user.IdentityId, createdGames, ownedGames);
        }
    }
}