using Application.Contracts.Infrastructure;
using Application.Genres.Queries;
using Application.Genres.Responses;
using Ardalis.Result;
using Mediator;

namespace Application.Genres.Handlers
{
    public class GetGenreByIdQueryHandler(IDatabase database)
        : IQueryHandler<GetGenreByIdQuery, Result<ApplicationGenre>>
    {
        public async ValueTask<Result<ApplicationGenre>> Handle(GetGenreByIdQuery query, CancellationToken cancellationToken)
        {
            var genre = await database.Genres.FindAsync(query.Id, cancellationToken);
            if (genre is null)
                return Result.NotFound();
            return Result.Success(ApplicationGenre.FromEntity(genre));
        }
    }
}