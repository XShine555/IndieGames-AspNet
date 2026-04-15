using Application.Games.Responses;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        ApplicationUserPicture? ProfilePicture,
        ICollection<ApplicationGame> CreatedGames,
        ICollection<ApplicationGame> OwnedGames,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}