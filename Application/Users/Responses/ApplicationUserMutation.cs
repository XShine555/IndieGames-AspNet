namespace Application.Users.Responses
{
    public record ApplicationUserMutation(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        DateTime UpdatedAt);
}
