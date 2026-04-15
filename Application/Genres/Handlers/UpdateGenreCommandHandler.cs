using Application.Abstractions.Common;
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
    public class UpdateGenreCommandHandler(
        IDatabase database,
        ILogger<UpdateGenreCommandHandler> logger)
        : ICommandHandler<UpdateGenreCommand, Result<ApplicationGenreMutation>>
    {
        public async ValueTask<Result<ApplicationGenreMutation>> Handle(UpdateGenreCommand command, CancellationToken cancellationToken)
        {
            var genre = await database.Genres
                .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);
            if (genre is null)
            {
                logger.LogWarning("Genre with id {Id} not found", command.Id);
                return Result.NotFound();
            }

            var nameResult = await UpdateName(command.Name, genre, cancellationToken);
            if (!nameResult.IsSuccess)
                return Result.Conflict(nameResult.Errors.FirstOrDefault());

            await database.SaveChangesAsync(cancellationToken);

            return Result.Success(new ApplicationGenreMutation(
                genre.Id,
                genre.Name,
                genre.UpdatedAt));
        }

        async Task<Result> UpdateName(string? name, Genre genre, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
                return Result.Success();

            var normalizedName = name.Trim().ToUpperInvariant();
            var existingGenre = await database.Genres.AsNoTracking()
                .AnyAsync(g => g.NormalizedName == normalizedName && g.Id != genre.Id, cancellationToken);
            if (existingGenre)
            {
                logger.LogWarning("Genre with name {Name} already exists", name);
                return Result.Conflict("Another genre with the same name already exists");
            }

            genre.Name = name.Trim();
            genre.NormalizedName = normalizedName;
            return Result.Success();
        }
    }
}
