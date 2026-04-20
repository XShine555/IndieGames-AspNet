using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Application.Users.Handlers
{
    public class GetUserCollectionByIdQueryHandler(
        IDatabase database,
        IGameCatalogMapper gameCatalogMapper)
        : IQueryHandler<GetUserCollectionByIdQuery, Result<ApplicationUserCollectionDetails>>
    {
        public async ValueTask<Result<ApplicationUserCollectionDetails>> Handle(GetUserCollectionByIdQuery query, CancellationToken cancellationToken)
        {
            var collection = await database.UserGameCollections
                .AsNoTracking()
                .AsSplitQuery()
                .Include(c => c.Items)
                    .ThenInclude(i => i.Game)
                        .ThenInclude(g => g.Owner)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Game)
                        .ThenInclude(g => g.Genres)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Game)
                        .ThenInclude(g => g.StorePictures)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Game)
                        .ThenInclude(g => g.Artworks)
                .SingleOrDefaultAsync(c => c.UserId == query.UserId && c.Id == query.CollectionId, cancellationToken);

            if (collection is null)
                return Result.NotFound();

            var games = collection.Items
                .OrderBy(item => item.AddedAt)
                .Select(item => gameCatalogMapper.ToApplicationGame(item.Game))
                .ToArray();

            return Result.Success(new ApplicationUserCollectionDetails(
                collection.Id,
                collection.Name,
                games,
                collection.CreatedAt,
                collection.UpdatedAt));
        }
    }
}
