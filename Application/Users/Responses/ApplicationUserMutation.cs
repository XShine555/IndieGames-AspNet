using Domain.Users.Enums;

namespace Application.Users.Responses
{
    public record ApplicationUserMutation(
        Guid IdentityId,
        string Username,
        string DisplayUsername,
        UserRole Role,
        DateTime UpdatedAt);
}
