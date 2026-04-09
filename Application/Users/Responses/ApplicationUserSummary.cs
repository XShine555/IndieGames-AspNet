using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserSummary(
        string IdentityId,
        string Username)
    {
        public static ApplicationUserSummary FromEntity(User user)
        {
            return new ApplicationUserSummary(
                user.IdentityId,
                user.Username);
        }
    }
}