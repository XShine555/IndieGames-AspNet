using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Domain.Entities;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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
            var pageInfo = new StaticPagedList<Guid>(Array.Empty<Guid>(), query.PageNumber, query.PageSize, totalCount);

            if (totalCount == 0)
            {
                return new PaginatedApplicationResponse<ApplicationUserCollectionListItem>(
                    Array.Empty<ApplicationUserCollectionListItem>(),
                    pageInfo.PageNumber,
                    pageInfo.PageSize,
                    pageInfo.PageCount,
                    pageInfo.TotalItemCount,
                    pageInfo.HasNextPage,
                    pageInfo.HasPreviousPage);
            }

            var collectionRows = await baseQuery
                .OrderBy(c => c.Name)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
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

            return new PaginatedApplicationResponse<ApplicationUserCollectionListItem>(
                collections,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }

        private record CollectionRow(
            UserGameCollection Collection,
            int GamesCount,
            string[] PreviewSmallKeys);
    }
}
