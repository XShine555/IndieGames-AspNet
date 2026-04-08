using Amazon.CognitoIdentityProvider.Model;

namespace Application.Users.Responses
{
    public record InfrastructureUser(
        string Username)
    {
        public static InfrastructureUser FromCognitoUser(UserType userType)
        {
            var username = userType.Attributes.SingleOrDefault(a => a.Name == "username")
                ?? throw new InvalidOperationException("Cognito user does not have a 'username' attribute.");

            return new InfrastructureUser(
                username.Value);
        }
    }
}