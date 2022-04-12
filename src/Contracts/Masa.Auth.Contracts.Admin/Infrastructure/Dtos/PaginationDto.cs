namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class PaginationDto<TEntity> where TEntity : class
{
    public long Total { get; }

    public List<TEntity> Items { get; }

    public PaginationDto()
    {
        Items = new List<TEntity>();
    }

    [JsonConstructor]
    public PaginationDto(long total, List<TEntity> items)
    {
        Total = total;
        Items = items;
    }
}
