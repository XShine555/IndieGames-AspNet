using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class ClearUserCartCommandHandler(
        IDatabase database,
        ILogger<ClearUserCartCommandHandler> logger)
        : ICommandHandler<ClearUserCartCommand, Result>
    {
        public async ValueTask<Result> Handle(ClearUserCartCommand command, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            await database.UserCartItems
                .Where(ci => ci.UserId == command.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            return Result.NoContent();
        }
    }
}
