using Application.Abstractions;
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
                .AnyAsync(u => u.IdentityId == command.identityId, cancellationToken);
            if (!existingUser)
            {
                logger.LogWarning("User with ID '{identityId}' not found.", command.identityId);
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
            var genres = await database.Genres
                .Where(g => command.Genres.Contains(g.Id))
                .ToListAsync(cancellationToken);
            if (genres.Count != command.Genres.Count)
            {
                logger.LogWarning("One or more genres not found for IDs: {GenreIds}.", string.Join(", ", command.Genres));
                return Result.Invalid(new ValidationError("One or more genres not found."));
            }

            var newGame = CreateGameCommand.ToEntity(command, genres);
            await database.Games.AddAsync(newGame, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);

            return Result.Created(ApplicationGame.FromEntity(newGame));
        }
    }
}
