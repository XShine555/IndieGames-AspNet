using Domain.Entities;

namespace Application.Users.Responses
{
    public record ApplicationUserCollection(
        Guid Id,
        string Name)
    {
        public static ApplicationUserCollection FromEntity(UserGameCollection userGameCollection)
        {
            return new ApplicationUserCollection(
                userGameCollection.Id,
                userGameCollection.Name
            );
        }
    }
}