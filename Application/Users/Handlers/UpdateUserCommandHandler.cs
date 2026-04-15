using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class UpdateUserCommandHandler(
        IDatabase database,
        IUserMapper userMapper,
        ILogger<UpdateUserCommandHandler> logger)
        : ICommandHandler<UpdateUserCommand, Result<ApplicationUserMutation>>
    {
        public async ValueTask<Result<ApplicationUserMutation>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .SingleOrDefaultAsync(u => u.IdentityId == command.IdentityId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with IdentityId {IdentityId} not found", command.IdentityId);
                return Result.NotFound("User not found");
            }

            UpdateDisplayUsername(command.NewDisplayUsername, user);

            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(userMapper.ToApplicationUserMutation(user));
        }

        void UpdateDisplayUsername(string newUsername, User user)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                return;

            user.DisplayUsername = newUsername;
            user.NormalizedDisplayUsername = newUsername.Trim().ToUpperInvariant();
            logger.LogInformation("Updated DisplayUsername to {DisplayUsername} and NormalizedDisplayUsername to {NormalizedDisplayUsername} for user with IdentityId {IdentityId}",
                user.DisplayUsername, user.NormalizedDisplayUsername, user.IdentityId);
        }
    }
}
