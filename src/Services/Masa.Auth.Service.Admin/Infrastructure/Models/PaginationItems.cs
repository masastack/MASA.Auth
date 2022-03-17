namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class PaginationItems<T> where T : class
{
    public long Total { get; set; }

    public int TotalPage { get; set; }

    public IEnumerable<T> Items { get; set; }

    public PaginationItems()
    {
        Items = new List<T>();
    }

    public PaginationItems(long total, int totalPage, IEnumerable<T> items)
    {
        Total = total;
        TotalPage = totalPage;
        Items = items;
    }
}
