namespace Masa.Auth.ApiGateways.Caller.Response;

public class PaginationItemsResponse<TEntity> where TEntity : class
{
    public long Total { get; }

    public long TotalPages { get; }

    public List<TEntity> Items { get; }

    public PaginationItemsResponse(long total, long totalPages, List<TEntity> items)
    {
        Total = total;
        TotalPages = totalPages;
        Items = items;
    }
}
