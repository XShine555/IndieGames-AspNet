using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Games.Handlers
{
    public class CreateGameCommandHandler(IDatabase database, ILogger<CreateGameCommandHandler> logger)
        : ICommandHandler<CreateGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(CreateGameCommand command, CancellationToken cancellationToken)
        {
            var existingUser = await database.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == command.UserId, cancellationToken);
            if (!existingUser)
            {
                logger.LogWarning("User with ID '{UserId}' not found.", command.UserId);
                return Result.NotFound();
            }

            var normalizedTitle = command.Title.Trim().ToUpperInvariant();
            var existingGame = await database.Games
                .AsNoTracking()
                .AnyAsync(g => g.Title.ToUpper() == normalizedTitle, cancellationToken);
            if (existingGame)
            {
                logger.LogWarning("A game with the title '{Title}' already exists.", command.Title);
                return Result.Conflict();
            }

            var newGame = CreateGameCommand.ToEntity(command);
            await database.Games.AddAsync(newGame, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return Result.Created(ApplicationGame.FromEntity(newGame));
        }
    }
}