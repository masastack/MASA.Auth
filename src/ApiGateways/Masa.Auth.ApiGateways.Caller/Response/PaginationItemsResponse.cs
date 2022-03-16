namespace Masa.Auth.ApiGateways.Caller.Response;

public class PaginationItemsResponse<TEntity> where TEntity : class
{
    public long Total { get; }

    public long TotalPages { get; }

    public IEnumerable<TEntity> Items { get; }

    public PaginationItemsResponse(long total, long totalPages, IEnumerable<TEntity> items)
    {
        Total = total;
        TotalPages = totalPages;
        Items = items;
    }
}
