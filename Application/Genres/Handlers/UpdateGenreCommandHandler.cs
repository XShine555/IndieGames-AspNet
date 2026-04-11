using Application.Abstractions;
using Application.Genres.Commands;
using Application.Genres.Responses;
using Ardalis.Result;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Genres.Handlers
{
    public class UpdateGenreCommandHandler(IDatabase database, ILogger<UpdateGenreCommandHandler> logger)
        : ICommandHandler<UpdateGenreCommand, Result<ApplicationGenre>>
    {
        public async ValueTask<Result<ApplicationGenre>> Handle(UpdateGenreCommand command, CancellationToken cancellationToken)
        {
            var genre = await database.Genres.AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);
            if (genre is null)
            {
                logger.LogWarning("Genre with id {Id} not found", command.Id);
                return Result.NotFound();
            }

            var nameResult = await UpdateName(command.Name, genre, cancellationToken);
            if (!nameResult.IsSuccess)
                return nameResult;

            return Result.Success(ApplicationGenre.FromEntity(genre));
        }

        async Task<Result> UpdateName(string? name, Genre genre, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Success();

            var normalizedName = name.Trim().ToUpperInvariant();
            var existingGenre = await database.Genres.AsNoTracking()
                .AnyAsync(g => g.NormalizedName == normalizedName, cancellationToken);
            if (existingGenre)
            {
                logger.LogWarning("Genre with name {Name} already exists", name);
                return Result.Conflict();
            }
            genre.Name = name;
            genre.NormalizedName = normalizedName;
            return Result.Success();
        }
    }
}
