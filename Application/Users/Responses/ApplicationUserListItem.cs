namespace Application.Users.Responses
{
    public record ApplicationUserListItem(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        ApplicationUserPicture? ProfilePicture,
        int CreatedGamesCount,
        int OwnedGamesCount,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
