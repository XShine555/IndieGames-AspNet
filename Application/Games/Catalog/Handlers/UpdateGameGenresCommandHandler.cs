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
    public class UpdateGameGenresCommandHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper,
        ILogger<UpdateGameGenresCommandHandler> logger)
        : ICommandHandler<UpdateGameGenresCommand, Result<ApplicationGameGenresMutation>>
    {
        public async ValueTask<Result<ApplicationGameGenresMutation>> Handle(UpdateGameGenresCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .Include(g => g.Genres)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for genres update", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId} and cannot update genres", command.IdentityId, command.GameId);
                return Result.Forbidden();
            }

            var existingGenres = await database.Genres
                .AsNoTracking()
                .Where(g => command.Genres.Contains(g.Id))
                .ToListAsync(cancellationToken);

            var notExistingGenres = command.Genres.Except(existingGenres.Select(g => g.Id)).ToList();
            if (notExistingGenres.Count > 0)
            {
                logger.LogWarning("Genres with ids {GenreIds} not found for game with id {GameId}", string.Join(", ", notExistingGenres), game.Id);
                return Result.NotFound($"Genres with ids {string.Join(", ", notExistingGenres)} not found");
            }

            game.Genres = existingGenres;
            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(gameCatalogMapper.ToApplicationGameGenresMutation(game));
        }
    }
}
