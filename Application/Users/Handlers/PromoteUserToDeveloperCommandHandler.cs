using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
using Domain.Users.Enums;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class PromoteUserToDeveloperCommandHandler(
        IDatabase database,
        IUserMapper userMapper,
        ILogger<PromoteUserToDeveloperCommandHandler> logger)
        : ICommandHandler<PromoteUserToDeveloperCommand, Result<ApplicationUserMutation>>
    {
        public async ValueTask<Result<ApplicationUserMutation>> Handle(PromoteUserToDeveloperCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .SingleOrDefaultAsync(u => u.IdentityId == command.IdentityId, cancellationToken);

            if (user is null)
            {
                logger.LogWarning("User with IdentityId {IdentityId} not found", command.IdentityId);
                return Result.NotFound("User not found");
            }

            if (user.Role == UserRole.Developer)
            {
                logger.LogWarning("User with IdentityId {IdentityId} is already a developer", command.IdentityId);
                return Result.Conflict("User is already a developer");
            }

            user.Role = UserRole.Developer;
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User with IdentityId {IdentityId} has been promoted to developer", command.IdentityId);

            return Result.Success(userMapper.ToApplicationUserMutation(user));
        }
    }
}
