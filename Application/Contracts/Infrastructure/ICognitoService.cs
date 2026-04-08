using Application.Users.Responses;
using Ardalis.Result;

namespace Application.Contracts.Infrastructure
{
    public interface ICognitoService
    {
        Task<Result<InfrastructureUser>> GetUserByIdentityIdAsync(string identityId, CancellationToken cancellationToken);
    }
}