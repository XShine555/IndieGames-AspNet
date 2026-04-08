using Application.Users.Responses;

namespace Application.Contracts.Infrastructure
{
    public interface ICognitoService
    {
        Task<InfrastructureUser?> GetUserByIdentityIdAsync(string identityId, CancellationToken cancellationToken);
    }
}