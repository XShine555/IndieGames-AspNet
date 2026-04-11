using Application.Abstractions;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class UpdateUserCommandHandler(IDatabase database, ILogger<UpdateUserCommandHandler> logger)
        : ICommandHandler<UpdateUserCommand, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users.AsNoTracking()
                .SingleOrDefaultAsync(u => u.IdentityId == command.IdentityId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with IdentityId {IdentityId} not found", command.IdentityId);
                return Result.NotFound("User not found");
            }

            UpdateDisplayUsername(command.NewDisplayUsername, user);

            database.Users.Update(user);
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(ApplicationUser.FromEntity(user));
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
