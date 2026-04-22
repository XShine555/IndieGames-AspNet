using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Application.Users.Handlers
{
    public class GetUserCollectionsQueryHandler(IDatabase database, IUserMapper userMapper)
        : IQueryHandler<GetUserCollectionsQuery, PaginatedApplicationResponse<ApplicationUserCollectionListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationUserCollectionListItem>> Handle(GetUserCollectionsQuery query, CancellationToken cancellationToken)
        {
            var baseQuery = database.UserGameCollections
                .AsNoTracking()
                .Where(c => c.UserId == query.UserId);

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var collectionRows = await baseQuery
                .OrderBy(c => c.Name)
                .Select(c => new CollectionRow(
                    c,
                    c.Items.Count,
                    c.Items
                        .OrderBy(i => i.AddedAt)
                        .Where(i => i.Game.IsPublished)
                        .Take(4)
                        .Select(i => i.Game.Artworks
                            .Where(a => a.Type == GameArtworkType.Main)
                            .Select(p => p.SmallRelativePath)
                            .First())
                        .ToArray()))
                .ToArrayAsync(cancellationToken);

            var collections = new ApplicationUserCollectionListItem[collectionRows.Length];

            for (var index = 0; index < collectionRows.Length; index++)
            {
                var row = collectionRows[index];

                collections[index] = userMapper.ToApplicationUserCollectionListItem(
                    row.Collection,
                    row.GamesCount,
                    row.PreviewSmallKeys);
            }

            var pagedList = collections.ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationUserCollectionListItem>.FromPagedList(pagedList);
        }

        private record CollectionRow(
            UserGameCollection Collection,
            int GamesCount,
            string[] PreviewSmallKeys);
    }
}
