using Application.Contracts.Infrastructure;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
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

            user.Username = command.Username;
            database.Users.Update(user);
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(ApplicationUser.FromEntity(user));
        }
    }
}