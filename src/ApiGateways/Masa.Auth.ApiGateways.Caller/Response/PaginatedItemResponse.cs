namespace Masa.Auth.ApiGateways.Caller.Response;

public class PaginatedItemResponse<TEntity> where TEntity : class
{
    public int PageIndex { get; }

    public int PageSize { get; }

    public long Count { get; }

    public long TotalPages { get; }

    public IEnumerable<TEntity> Items { get; }

    public PaginatedItemResponse(
        int pageIndex,
        int pageSize,
        long count,
        long totalPages,
        IEnumerable<TEntity> items)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Count = count;
        TotalPages = totalPages;
        Items = items;
    }
}
