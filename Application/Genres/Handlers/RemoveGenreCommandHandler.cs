using Application.Abstractions;
using Application.Genres.Commands;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Genres.Handlers
{
    public class RemoveGenreCommandHandler(IDatabase database, ILogger<RemoveGenreCommandHandler> logger)
        : ICommandHandler<RemoveGenreCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGenreCommand command, CancellationToken cancellationToken)
        {
            var genre = await database.Genres.AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == command.Id, cancellationToken);
            if (genre is null)
            {
                logger.LogWarning("Genre with id {Id} not found", command.Id);
                return Result.NotFound();
            }

            database.Genres.Remove(genre);
            await database.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Genre with id {Id} removed successfully", command.Id);
            return Result.Success();
        }
    }
}
