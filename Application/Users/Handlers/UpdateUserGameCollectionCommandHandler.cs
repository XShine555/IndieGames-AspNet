using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class UpdateUserGameCollectionCommandHandler(
        IDatabase database,
        ILogger<UpdateUserGameCollectionCommandHandler> logger)
        : ICommandHandler<UpdateUserGameCollectionCommand, Result>
    {
        public async ValueTask<Result> Handle(UpdateUserGameCollectionCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Invalid(new ValidationError("Collection name is required."));

            var collection = await database.UserGameCollections
                .SingleOrDefaultAsync(c => c.Id == command.CollectionId, cancellationToken);
            if (collection is null)
            {
                logger.LogWarning("Collection with id {CollectionId} not found", command.CollectionId);
                return Result.NotFound("Collection not found");
            }

            if (collection.UserId != command.UserId)
            {
                logger.LogWarning("User with id {UserId} cannot update collection {CollectionId}", command.UserId, command.CollectionId);
                return Result.Forbidden();
            }

            var normalizedName = command.Name.Trim().ToUpperInvariant();
            var duplicateName = await database.UserGameCollections
                .AsNoTracking()
                .AnyAsync(c => c.UserId == command.UserId
                               && c.NormalizedName == normalizedName
                               && c.Id != command.CollectionId,
                    cancellationToken);
            if (duplicateName)
                return Result.Conflict("A collection with the same name already exists");

            collection.Name = command.Name.Trim();
            collection.NormalizedName = normalizedName;
            collection.UpdatedAt = DateTime.UtcNow;

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
