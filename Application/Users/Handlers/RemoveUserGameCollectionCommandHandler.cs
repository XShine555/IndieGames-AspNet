using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class RemoveUserGameCollectionCommandHandler(
        IDatabase database,
        ILogger<RemoveUserGameCollectionCommandHandler> logger)
        : ICommandHandler<RemoveUserGameCollectionCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveUserGameCollectionCommand command, CancellationToken cancellationToken)
        {
            var collection = await database.UserGameCollections
                .SingleOrDefaultAsync(c => c.Id == command.CollectionId, cancellationToken);
            if (collection is null)
            {
                logger.LogWarning("Collection with id {CollectionId} not found", command.CollectionId);
                return Result.NotFound("Collection not found");
            }

            if (collection.UserId != command.UserId)
            {
                logger.LogWarning("User with id {UserId} cannot delete collection {CollectionId}", command.UserId, command.CollectionId);
                return Result.Forbidden();
            }

            database.UserGameCollections.Remove(collection);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
