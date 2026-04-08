using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Application.Contracts.Infrastructure;
using Application.Users.Responses;
using Ardalis.Result;
using Infrastructure.Configurations;

namespace Infrastructure.Services
{
    public class CognitoService(IAmazonCognitoIdentityProvider amazonCognitoIdentityProvider, CognitoConfiguration cognitoConfiguration)
        : ICognitoService
    {
        public async Task<Result<InfrastructureUser>> GetUserByIdentityIdAsync(string identityId, CancellationToken cancellationToken)
        {
            var request = new ListUsersRequest
            {
                UserPoolId = cognitoConfiguration.UserPoolId,
                Filter = $"sub = \"{identityId}\"",
                Limit = 1
            };
            var response = await amazonCognitoIdentityProvider.ListUsersAsync(request);
            var user = response.Users.SingleOrDefault();

            if (user is null)
                return Result.NotFound();

            return InfrastructureUser.FromCognitoUser(user);
        }
    }
}