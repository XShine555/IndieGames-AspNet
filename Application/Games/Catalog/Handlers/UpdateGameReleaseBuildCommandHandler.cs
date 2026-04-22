using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Catalog.Commands;
using Application.Games.Catalog.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Catalog.Handlers
{
    public class UpdateGameReleaseBuildCommandHandler(IDatabase database, ILogger<UpdateGameReleaseBuildCommandHandler> logger, IGameCatalogMapper mapper)
        : ICommandHandler<UpdateGameReleaseBuildCommand, Result<ApplicationGameMutation>>
    {
        public async ValueTask<Result<ApplicationGameMutation>> Handle(UpdateGameReleaseBuildCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .Include(g => g.Builds)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound("Game not found");
            }

            if (game.OwnerId != command.UserId)
            {
                logger.LogWarning("User with id {UserId} is not the owner of the game with id {GameId}", command.UserId, command.GameId);
                return Result.Unauthorized();
            }

            var buildExists = game.Builds.Any(b => b.Id == command.BuildId);
            if (!buildExists)
            {
                logger.LogWarning("Build with id {BuildId} not found for game with id {GameId}", command.BuildId, command.GameId);
                return Result.NotFound("Build not found for the game");
            }

            game.ReleaseGameBuildId = command.BuildId;
            database.Games.Update(game);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Success(mapper.ToApplicationGameMutation(game));
        }
    }
}