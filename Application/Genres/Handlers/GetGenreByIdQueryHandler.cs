using Application.Abstractions;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Genres.Handlers
{
    public class GetGenreByIdQueryHandler(IDatabase database)
        : IQueryHandler<GetGenreByIdQuery, Result<ApplicationGenre>>
    {
        public async ValueTask<Result<ApplicationGenre>> Handle(GetGenreByIdQuery query, CancellationToken cancellationToken)
        {
            var genre = await database.Genres.AsNoTracking()
                .SingleOrDefaultAsync(g => g.Id == query.Id, cancellationToken);
            if (genre is null)
                return Result.NotFound();
            return Result.Success(ApplicationGenre.FromEntity(genre));
        }
    }
}