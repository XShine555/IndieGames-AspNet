using Application.Abstractions.Common;
using Application.Abstractions.Persistence;
using Application.Users.Queries;
using Application.Users.Responses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace Application.Users.Handlers
{
    public class GetUsersQueryHandler(
        IDatabase database,
        IUserMapper userMapper)
        : IQueryHandler<GetUsersQuery, PaginatedApplicationResponse<ApplicationUserListItem>>
    {
        public async ValueTask<PaginatedApplicationResponse<ApplicationUserListItem>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            var normalizedDisplayName = query.DisplayName?.Trim().ToUpperInvariant();
            var hasDisplayNameFilter = !string.IsNullOrWhiteSpace(normalizedDisplayName);

            var baseQuery = database.Users
                .AsNoTracking()
                .AsQueryable();

            if (hasDisplayNameFilter)
            {
                baseQuery = baseQuery.Where(u => u.NormalizedDisplayUsername.Contains(normalizedDisplayName!));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var users = await baseQuery
                .OrderBy(u => u.DisplayUsername)
                .Select(userMapper.ToApplicationUserListItemExpression())
                .ToListAsync(cancellationToken);

            var pagedList = users.ToPagedList(query.PageNumber, query.PageSize, totalCount);
            return PaginatedApplicationResponse<ApplicationUserListItem>.FromPagedList(pagedList);
        }
    }
}
