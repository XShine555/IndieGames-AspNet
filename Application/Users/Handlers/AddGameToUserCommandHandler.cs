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
    public class AddGameToUserCommandHandler(IDatabase database, ILogger<AddGameToUserCommandHandler> logger)
         : ICommandHandler<AddGameToUserCommand, Result<ApplicationUser>>
    {
        public async ValueTask<Result<ApplicationUser>> Handle(AddGameToUserCommand command, CancellationToken cancellationToken)
        {
            var user = await database.Users
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Pictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Pictures)
                .SingleOrDefaultAsync(u => u.IdentityId == command.UserId, cancellationToken);
            if (user is null)
            {
                logger.LogWarning("User with id {UserId} not found", command.UserId);
                return Result.NotFound("User not found");
            }

            var game = await database.Games.FindAsync(command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            var ownedGame = user.OwnedGames.Any(ug => ug.GameId == command.GameId);
            if (ownedGame)
            {
                logger.LogInformation("User with id {UserId} already owns game with id {GameId}", command.UserId, command.GameId);
                return Result.Conflict("User already owns this game");
            }

            await database.UserOwnedGames.AddAsync(new UserOwnedGame
            {
                UserId = user.IdentityId,
                GameId = game.Id
            }, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            var refreshedUser = await database.Users
                .AsNoTracking()
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Genres)
                .Include(u => u.CreatedGames)
                    .ThenInclude(g => g.Pictures)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Genres)
                .Include(u => u.OwnedGames)
                    .ThenInclude(ug => ug.Game)
                        .ThenInclude(g => g.Pictures)
                .SingleAsync(u => u.IdentityId == command.UserId, cancellationToken);

            return Result.Success(ApplicationUser.FromEntity(refreshedUser));
        }
    }
}
