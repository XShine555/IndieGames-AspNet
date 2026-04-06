using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class DeleteGameUsersCommandHandler(
        IDatabase database,
        ILogger<DeleteGameUsersCommandHandler> logger)
        : ICommandHandler<DeleteGameUsersCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(DeleteGameUsersCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for users deletion", command.GameId);
                return Result.NotFound();
            }

            var requestedUserIds = command.Users.ToArray();
            if (requestedUserIds.Length != requestedUserIds.Distinct().Count())
            {
                logger.LogWarning("Duplicate user ids were provided for game with id {GameId}", command.GameId);
                return Result.Conflict("Duplicate user ids are not allowed");
            }

            var existingUserIds = await database.Users
                .AsNoTracking()
                .Where(u => requestedUserIds.Contains(u.Id))
                .Select(u => u.Id)
                .ToArrayAsync(cancellationToken);

            var missingUserIds = requestedUserIds.Except(existingUserIds).ToArray();
            if (missingUserIds.Length > 0)
            {
                logger.LogWarning(
                    "Some users were not found when deleting from game with id {GameId}. Missing users: {MissingUserIds}",
                    command.GameId,
                    string.Join(", ", missingUserIds));
                return Result.NotFound($"The following users were not found: {string.Join(", ", missingUserIds)}");
            }

            var currentUserIds = await database.UsersToGames
                .AsNoTracking()
                .Where(utg => utg.GameId == game.Id)
                .Select(utg => utg.UserId)
                .ToArrayAsync(cancellationToken);

            var notAssociatedUserIds = requestedUserIds.Except(currentUserIds).ToArray();
            if (notAssociatedUserIds.Length > 0)
            {
                logger.LogWarning(
                    "Some users are not associated with game id {GameId}: {UserIds}",
                    command.GameId,
                    string.Join(", ", notAssociatedUserIds));
                return Result.NotFound($"The following users are not associated with the game: {string.Join(", ", notAssociatedUserIds)}");
            }

            var usersToRemove = await database.UsersToGames
                .Where(utg => utg.GameId == game.Id && requestedUserIds.Contains(utg.UserId))
                .ToArrayAsync(cancellationToken);
            database.UsersToGames.RemoveRange(usersToRemove);

            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Deleted users from game with id {GameId}", command.GameId);
            return Result.Success(await LoadGameAsync(game.Id, cancellationToken));
        }

        async Task<ApplicationGame> LoadGameAsync(Guid gameId, CancellationToken cancellationToken)
        {
            var updatedGame = await database.Games
                .Where(g => g.Id == gameId)
                .Include(g => g.Genres)
                .Include(g => g.UsersToGames).ThenInclude(utg => utg.User)
                .Include(g => g.UsersToGameRequests).ThenInclude(utgr => utgr.User)
                .SingleAsync(cancellationToken);

            return ApplicationGame.FromEntity(updatedGame);
        }
    }
}
