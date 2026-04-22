using Domain.Users.Enums;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        UserRole Role,
        ApplicationUserPicture ProfilePicture,
        ICollection<ApplicationUserGame> CreatedGames,
        ICollection<ApplicationUserGame> OwnedGames,
        ICollection<ApplicationUserCartItem> CartItems,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}