using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUser(
        long Id,
        string IdentityId)
    {
        public static ApplicationUser FromEntity(User user)
        {
            return new ApplicationUser(
                user.Id,
                user.IdentityId);
        }
    }
}