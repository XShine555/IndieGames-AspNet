using Application.Users.Responses;
using Domain.Entities;

namespace Application.Abstractions.Common
{
    public interface IUserMapper
    {
        ApplicationUser ToApplicationUser(User user);
    }
}
