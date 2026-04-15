using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class AddGameToUserCollectionCommandHandler(
        IDatabase database,
        ILogger<AddGameToUserCollectionCommandHandler> logger)
        : ICommandHandler<AddGameToUserCollectionCommand, Result>
    {
        public async ValueTask<Result> Handle(AddGameToUserCollectionCommand command, CancellationToken cancellationToken)
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
                logger.LogWarning("User with id {UserId} cannot add game to collection {CollectionId}", command.UserId, command.CollectionId);
                return Result.Forbidden();
            }

            var ownedGame = await database.UserLibrary
                .AsNoTracking()
                .AnyAsync(ug => ug.UserId == command.UserId && ug.GameId == command.GameId, cancellationToken);
            if (!ownedGame)
            {
                logger.LogWarning("User with id {UserId} does not own game {GameId} and cannot add it to collection {CollectionId}",
                    command.UserId,
                    command.GameId,
                    command.CollectionId);
                return Result.Conflict("The game must be in the user library before adding it to a collection");
            }

            var alreadyInCollection = await database.UserGameCollectionItems
                .AsNoTracking()
                .AnyAsync(item => item.CollectionId == command.CollectionId && item.GameId == command.GameId, cancellationToken);
            if (alreadyInCollection)
                return Result.Conflict("The game is already in the collection");

            await database.UserGameCollectionItems.AddAsync(new UserGameCollectionItem
            {
                CollectionId = command.CollectionId,
                GameId = command.GameId
            }, cancellationToken);

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
