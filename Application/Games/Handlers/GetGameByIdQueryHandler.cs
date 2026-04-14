using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Games.Queries;
using Application.Games.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Games.Handlers
{
    public class GetGameByIdQueryHandler(
        IDatabase database,
        IGameMapper gameMapper)
        : IQueryHandler<GetGameByIdQuery, Result<ApplicationGame>>
    {
        public async ValueTask<Result<ApplicationGame>> Handle(GetGameByIdQuery query, CancellationToken cancellationToken)
        {
            var game = await database.Games
                .AsNoTracking()
                .AsSplitQuery()
                .Include(g => g.Owner)
                .Include(g => g.Genres)
                .Include(g => g.StorePictures)
                .Include(g => g.Artworks)
                .SingleOrDefaultAsync(q => q.Id == query.Id, cancellationToken);

            if (game is null)
                return Result.NotFound();

            return Result.Success(gameMapper.ToApplicationGame(game));
        }
    }
}
