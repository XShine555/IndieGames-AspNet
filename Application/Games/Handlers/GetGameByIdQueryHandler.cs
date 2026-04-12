using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Games.Handlers
{
    public class GetGameByIdQueryHandler(IDatabase database)
        : IQueryHandler<GetGameByIdQuery, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games.AsNoTracking()
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .SingleOrDefaultAsync(q => q.Id == query.Id, cancellationToken);
            if (game is null)
                return Result.NotFound();

            return Result.Success(ApplicationGame.FromEntity(game));
        }
    }
}
