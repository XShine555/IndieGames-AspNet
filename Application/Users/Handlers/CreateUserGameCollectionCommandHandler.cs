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
    public class CreateUserGameCollectionCommandHandler(
        IDatabase database,
        ILogger<CreateUserGameCollectionCommandHandler> logger)
        : ICommandHandler<CreateUserGameCollectionCommand, Result<ApplicationUserCollection>>
    {
        public async ValueTask<Result<ApplicationUserCollection>> Handle(CreateUserGameCollectionCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Invalid(new ValidationError("Collection name is required."));

            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var normalizedName = command.Name.Trim().ToUpperInvariant();
            var duplicateName = await database.UserGameCollections
                .AsNoTracking()
                .AnyAsync(c => c.UserId == command.UserId && c.NormalizedName == normalizedName, cancellationToken);
            if (duplicateName)
            {
                logger.LogInformation("Collection with name {CollectionName} already exists for user {UserId}", command.Name, command.UserId);
                return Result.Conflict("A collection with the same name already exists");
            }

            var collection = new UserGameCollection
            {
                UserId = command.UserId,
                Name = command.Name.Trim(),
                NormalizedName = normalizedName,
            };

            await database.UserGameCollections.AddAsync(collection, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return Result.Created(ApplicationUserCollection.FromEntity(collection));
        }
    }
}
