using Domain.Users.Enums;

namespace Application.Users.Responses
{
    public record ApplicationBasicUser(
        Guid Id,
        string DisplayName,
        ApplicationUserPicture ProfilePicture,
        UserRole Role);
}