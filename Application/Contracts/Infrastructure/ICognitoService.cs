using Application.Users.Responses;

namespace Application.Contracts.Infrastructure
{
    public interface ICognitoService
    {
        Task<InfrastructureUser?> GetUser(string identityId, CancellationToken cancellationToken);
    }
}