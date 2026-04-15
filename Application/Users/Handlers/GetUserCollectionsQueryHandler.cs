using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Application.Users.Handlers
{
    public class GetUserCollectionsQueryHandler(IDatabase database)
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

            var collections = await baseQuery
                .OrderBy(c => c.Name)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(c => new ApplicationUserCollectionListItem(
                    c.Id,
                    c.Name,
                    c.Items.Count,
                    c.CreatedAt,
                    c.UpdatedAt))
                .ToArrayAsync(cancellationToken);

            return new PaginatedApplicationResponse<ApplicationUserCollectionListItem>(
                collections,
                pageInfo.PageNumber,
                pageInfo.PageSize,
                pageInfo.PageCount,
                pageInfo.TotalItemCount,
                pageInfo.HasNextPage,
                pageInfo.HasPreviousPage);
        }
    }
}
