using Amazon.CognitoIdentityProvider.Model;

namespace Application.Users.Responses
{
    public record InfrastructureUser(
        string Username)
    {
        public static InfrastructureUser FromCognitoUser(UserType userType)
        {
            return new InfrastructureUser(
                userType.Username);
        }
    }
}