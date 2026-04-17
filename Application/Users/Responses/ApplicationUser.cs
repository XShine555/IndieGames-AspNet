using Application.Games.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        ApplicationUserPicture ProfilePicture,
        ICollection<ApplicationUserGame> CreatedGames,
        ICollection<ApplicationUserGame> OwnedGames,
        ICollection<ApplicationUserCartItem> CartItems,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}