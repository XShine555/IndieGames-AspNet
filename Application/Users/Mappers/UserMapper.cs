using Application.Abstractions.Common;
using Application.Users.Responses;
using Domain.Entities;

namespace Application.Users.Mappers
{
    public class UserMapper(IGameMapper gameMapper) : IUserMapper
    {
        public ApplicationUser ToApplicationUser(User user)
        {
            var createdGames = user.CreatedGames
                .Select(gameMapper.ToApplicationGame)
                .ToArray();

            var ownedGames = user.OwnedGames
                .Select(ownedGame => gameMapper.ToApplicationGame(ownedGame.Game))
                .ToArray();

            return new ApplicationUser(
                user.IdentityId,
                user.Username,
                user.DisplayUsername,
                ApplicationUserPicture.FromEntity(user.ProfilePicture),
                createdGames,
                ownedGames,
                user.CreatedAt,
                user.UpdatedAt);
        }
    }
}
