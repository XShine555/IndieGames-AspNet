using Application.Contracts.Application;
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
    public class UpdateGameCommandHandler(IDatabase database, ILogger<UpdateGameCommandHandler> logger,
        IGamePicturesHelper gamePicturesHelper)
        : ICommandHandler<UpdateGameCommand, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(UpdateGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .Include(g => g.Genres)
                .Include(g => g.Pictures)
                .SingleOrDefaultAsync(g => g.Id == command.GameId, cancellationToken);
            if (game is null)
            {
                logger.LogWarning("Game with id {GameId} not found for update", command.GameId);
                return Result.NotFound();
            }

            if (game.OwnerId != command.IdentityId)
            {
                logger.LogWarning("User with id {IdentityId} is not the owner of game with id {GameId} and cannot update it", command.IdentityId, command.GameId);
                return Result.Forbidden();
            }

            var titleResult = await UpdateTitle(command.Title, game, cancellationToken);
            if (!titleResult.IsSuccess)
                return titleResult;

            UpdateDescription(command.Description, game);

            var ownerResult = await UpdateOwnerId(command.OwnerId, game, cancellationToken);
            if (!ownerResult.IsSuccess)
                return ownerResult;

            var genresResult = await UpdateGenres(command.Genres, game, cancellationToken);
            if (!genresResult.IsSuccess)
                return genresResult;

            await database.SaveChangesAsync(cancellationToken);
            var pictures = await gamePicturesHelper.GetPictures(game, cancellationToken);
            return Result.Success(ApplicationGame.FromEntity(game, pictures));
        }

        async Task<Result> UpdateTitle(string? newTitle, Game game, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                return Result.Success();

            var trimmedTitle = newTitle.Trim();
            var normalizedTitle = trimmedTitle.ToUpperInvariant();
            var existingGame = await database.Games
                .SingleOrDefaultAsync(g => g.NormalizedTitle == normalizedTitle && g.Id != game.Id, cancellationToken);
            if (existingGame is not null)
            {
                logger.LogWarning("Game with title {GameTitle} already exists with id {ExistingGameId}", newTitle, existingGame.Id);
                return Result.Conflict("Another Game with the same title already exists");
            }

            game.Title = trimmedTitle;
            game.NormalizedTitle = normalizedTitle;

            logger.LogInformation("Updating game title for game with id {GameId}", game.Id);
            return Result.Success();
        }

        void UpdateDescription(string? newDescription, Game game)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                return;

            logger.LogInformation("Updating game description for game with id {GameId}", game.Id);
            game.Description = newDescription;
        }

        async Task<Result> UpdateOwnerId(string? ownerId, Game game, CancellationToken cancellationToken)
        {
            var newOwner = await database.Users.SingleOrDefaultAsync(u => u.IdentityId == ownerId, cancellationToken);
            if (newOwner is null)
            {
                logger.LogWarning("User with id {OwnerId} not found for game with id {GameId}", ownerId, game.Id);
                return Result.NotFound($"User with id {ownerId} not found");
            }

            game.OwnerId = newOwner.IdentityId;
            return Result.Success();
        }

        async Task<Result> UpdateGenres(ICollection<int> genres, Game game, CancellationToken cancellationToken)
        {
            var existingGenres = await database.Genres.AsNoTracking()
                .Where(g => genres.Contains(g.Id))
                .ToListAsync(cancellationToken);
            var notExistingGenres = genres.Except(existingGenres.Select(g => g.Id)).ToList();
            if (notExistingGenres.Count > 0)
            {
                logger.LogWarning("Genres with ids {GenreIds} not found for game with id {GameId}", notExistingGenres, game.Id);
                return Result.NotFound($"Genres with ids {string.Join(", ", notExistingGenres) } not found");
            }

            logger.LogInformation("Updating game genres for game with id {GameId}", game.Id);
            game.Genres = existingGenres;
            return Result.Success();
        }
    }
}