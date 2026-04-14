using Application.Abstractions.Persistence;
using Application.Users.Commands;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Users.Handlers
{
    public class AddGameToUserLibraryCommandHandler(
        IDatabase database,
        ILogger<AddGameToUserLibraryCommandHandler> logger)
        : ICommandHandler<AddGameToUserLibraryCommand, Result>
    {
        public async ValueTask<Result> Handle(AddGameToUserLibraryCommand command, CancellationToken cancellationToken)
        {
            var userExists = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (!userExists)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var gameExists = await database.Games
                .AsNoTracking()
                .AnyAsync(g => g.Id == command.GameId, cancellationToken);
            if (!gameExists)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            var alreadyOwned = await database.UserOwnedGames
                .AsNoTracking()
                .AnyAsync(ug => ug.UserId == command.UserId && ug.GameId == command.GameId, cancellationToken);
            if (alreadyOwned)
            {
                logger.LogInformation("User with id {UserId} already owns game with id {GameId}", command.UserId, command.GameId);
                return Result.Conflict("User already owns this game");
            }

            await database.UserOwnedGames.AddAsync(new UserOwnedGame
            {
                UserId = command.UserId,
                GameId = command.GameId
            }, cancellationToken);

            await database.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
