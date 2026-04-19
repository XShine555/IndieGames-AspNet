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
    public class CreateGameBuildCommandHandler(IDatabase database, IGameMapper gameMapper, ILogger<CreateGameBuildCommandHandler> logger)
         : ICommandHandler<CreateGameBuildCommand, Result<ApplicationGameBuildMutation>>
    {
        public async ValueTask<Result<ApplicationGameBuildMutation>> Handle(CreateGameBuildCommand command, CancellationToken cancellationToken)
        {
            var existingGame = await database.Games
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (existingGame is null)
            {
                logger.LogWarning("Game with id {GameId} not found", command.GameId);
                return Result.NotFound();
            }
            if (existingGame.OwnerId != command.UserId)
            {
                logger.LogWarning("User with id {UserId} is not the owner of existingGame with id {GameId}", command.UserId, command.GameId);
                return Result.Forbidden();
            }

            var newGameBuild = CreateGameBuildCommand.ToEntity(command);
            await database.GameBuilds.AddAsync(newGameBuild, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            return Result.Created(gameMapper.ToApplicationGameBuildMutation(newGameBuild));
        }
    }
}