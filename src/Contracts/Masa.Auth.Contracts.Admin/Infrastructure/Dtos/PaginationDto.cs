namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class PaginationDto<TEntity> where TEntity : class
{
    public long Total { get; }

    public int TotalPages { get; }

    public List<TEntity> Items { get; }

    public PaginationDto()
    {
        Items = new List<TEntity>();
    }

    public PaginationDto(long total, int totalPages, List<TEntity> items)
    {
        Total = total;
        TotalPages = totalPages;
        Items = items;
    }
}
