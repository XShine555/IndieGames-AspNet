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
    public class AddGameRequestedUsersCommandHandler(
        IDatabase database,
        ILogger<AddGameRequestedUsersCommandHandler> logger)
        : ICommandHandler<AddGameRequestedUsersCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(AddGameRequestedUsersCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for requested users addition", command.GameId);
                return Result.NotFound();
            }

            var requestedUserIds = command.RequestedUsers.ToArray();
            if (requestedUserIds.Length != requestedUserIds.Distinct().Count())
            {
                logger.LogWarning("Duplicate requested user ids were provided for game with id {GameId}", command.GameId);
                return Result.Conflict("Duplicate requested user ids are not allowed");
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
                    "Some requested users were not found when adding to game with id {GameId}. Missing users: {MissingUserIds}",
                    command.GameId,
                    string.Join(", ", missingUserIds));
                return Result.NotFound($"The following users were not found: {string.Join(", ", missingUserIds)}");
            }

            var currentRequestedUserIds = await database.UsersToGameRequests
                .AsNoTracking()
                .Where(utgr => utgr.GameId == game.Id)
                .Select(utgr => utgr.UserId)
                .ToArrayAsync(cancellationToken);

            var alreadyAssociatedUserIds = requestedUserIds.Intersect(currentRequestedUserIds).ToArray();
            if (alreadyAssociatedUserIds.Length > 0)
            {
                logger.LogWarning(
                    "Some requested users are already associated with game id {GameId}: {UserIds}",
                    command.GameId,
                    string.Join(", ", alreadyAssociatedUserIds));
                return Result.Conflict($"The following users are already associated with the game request list: {string.Join(", ", alreadyAssociatedUserIds)}");
            }

            foreach (var userId in requestedUserIds)
                database.UsersToGameRequests.Add(new UserToGameRequest { GameId = game.Id, UserId = userId });

            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Added requested users for game with id {GameId}", command.GameId);
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
