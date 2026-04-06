using Application.Contracts.Infrastructure;
using Application.Users.Commands;
using Application.Users.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class CreateUserCommandHandler(IDatabase database, ILogger<CreateUserCommandHandler> logger)
        : ICommandHandler<CreateUserCommand, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var anyUser = await database.Users.AnyAsync(u => u.IdentityId == command.IdentityId, cancellationToken);

            if (anyUser)
            {
                logger.LogWarning("User with identity id {IdentityId} already exists", command.IdentityId);
                return Result.Conflict();
            }

            var newUser = new User
            {
                IdentityId = command.IdentityId,
            };
            await database.Users.AddAsync(newUser, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new user with id {UserId} and identity id {IdentityId}", newUser.Id, newUser.IdentityId);

            return Result.Created(ApplicationUser.FromEntity(newUser));
        }
    }
}