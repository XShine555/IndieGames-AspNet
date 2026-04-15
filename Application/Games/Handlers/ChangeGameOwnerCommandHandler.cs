using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class ChangeGameOwnerCommandHandler(
        IDatabase database,
        ILogger<ChangeGameOwnerCommandHandler> logger)
        : ICommandHandler<ChangeGameOwnerCommand, Result<ApplicationGameMutation>>
    {
        public async ValueTask<Result<ApplicationGameMutation>> Handle(ChangeGameOwnerCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for owner change", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId} and cannot change owner", command.IdentityId, command.GameId);
                return Result.Forbidden();
            }

            var newOwner = await database.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(user => user.IdentityId == command.NewOwnerId, cancellationToken);

            if (newOwner is null)
            {
                logger.LogWarning("User with id {NewOwnerId} not found for owner change in game {GameId}", command.NewOwnerId, command.GameId);
                return Result.NotFound($"User with id {command.NewOwnerId} not found");
            }

            game.OwnerId = newOwner.IdentityId;
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(new ApplicationGameMutation(
                game.Id,
                game.OwnerId,
                game.Title,
                game.Price,
                game.Discount,
                game.IsPublic,
                game.IsPublished,
                game.UpdatedAt));
        }
    }
}
