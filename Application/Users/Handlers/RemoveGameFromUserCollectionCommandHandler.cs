using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class RemoveGameFromUserCollectionCommandHandler(
        IDatabase database,
        ILogger<RemoveGameFromUserCollectionCommandHandler> logger)
        : ICommandHandler<RemoveGameFromUserCollectionCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameFromUserCollectionCommand command, CancellationToken cancellationToken)
        {
            var collection = await database.UserGameCollections
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == command.CollectionId, cancellationToken);
            if (collection is null)
            {
                logger.LogWarning("Collection with id {CollectionId} not found", command.CollectionId);
                return Result.NotFound("Collection not found");
            }

            if (collection.UserId != command.UserId)
            {
                logger.LogWarning("User with id {UserId} cannot remove game from collection {CollectionId}", command.UserId, command.CollectionId);
                return Result.Forbidden();
            }

            var item = await database.UserGameCollectionItems
                .SingleOrDefaultAsync(x => x.CollectionId == command.CollectionId && x.GameId == command.GameId, cancellationToken);
            if (item is null)
            {
                logger.LogWarning("Game {GameId} is not present in collection {CollectionId}", command.GameId, command.CollectionId);
                return Result.NotFound("Game not found in collection");
            }

            database.UserGameCollectionItems.Remove(item);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
