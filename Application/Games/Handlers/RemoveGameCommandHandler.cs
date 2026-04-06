using Application.Contracts.Infrastructure;
using Application.Games.Commands;
using Ardalis.Result;
using Mediator;

namespace Application.Games.Handlers
{
    public class RemoveGameCommandHandler(IDatabase database)
        : ICommandHandler<RemoveGameCommand, Result>
    {
        public async ValueTask<Result> Handle(RemoveGameCommand command, CancellationToken cancellationToken)
        {
            var game = await database.Games.FindAsync(command.Id, cancellationToken);
            if (game is null)
                return Result.NotFound();

            database.Games.Remove(game);
            await database.SaveChangesAsync(cancellationToken);
            return Result.NoContent();
        }
    }
}