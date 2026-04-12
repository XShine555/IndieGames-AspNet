using Application.Abstractions.Persistence;
using Application.Genres.Commands;
using Application.Genres.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Genres.Handlers
{
    public class CreateGenreCommandHandler(IDatabase database, ILogger<CreateGenreCommandHandler> logger)
        : ICommandHandler<CreateGenreCommand, Result<ApplicationGenre>>
    {
        public async ValueTask<Result<ApplicationGenre>> Handle(CreateGenreCommand command, CancellationToken cancellationToken)
        {
            var normalizedName = command.Name.Trim().ToUpperInvariant();
            var existingGenre = await database.Genres.AsNoTracking().AnyAsync(g => g.NormalizedName == normalizedName, cancellationToken);
            if (existingGenre)
            {
                logger.LogWarning("Genre with name '{GenreName}' already exists.", command.Name);
                return Result.Conflict();
            }

            var newGenre = new Genre
            {
                Name = command.Name,
                NormalizedName = normalizedName,
            };
            await database.Genres.AddAsync(newGenre, cancellationToken);
            await database.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Created new genre with ID '{GenreId}' and name '{GenreName}'.", newGenre.Id, newGenre.Name);

            return Result.Success();
        }
    }
}
