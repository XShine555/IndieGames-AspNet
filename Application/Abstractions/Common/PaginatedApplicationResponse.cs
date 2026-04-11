using X.PagedList;

namespace Application.Abstractions
{
    public record PaginatedApplicationResponse<T>(
        IReadOnlyCollection<T> Items,
        int PageNumber,
        int PageSize,
        int PageCount,
        int TotalItemCount,
        bool HasNextPage,
        bool HasPreviousPage)
    {
        public static PaginatedApplicationResponse<T> FromPagedList(IPagedList<T> pagedList)
        {
            return new PaginatedApplicationResponse<T>(
                Items: pagedList.ToArray(),
                PageNumber: pagedList.PageNumber,
                PageSize: pagedList.PageSize,
                PageCount: pagedList.PageCount,
                TotalItemCount: pagedList.TotalItemCount,
                HasNextPage: pagedList.HasNextPage,
                HasPreviousPage: pagedList.HasPreviousPage);
        }
    }
}
