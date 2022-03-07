namespace MASA.Auth.Service.Infrastructure.Models;

public class PaginationList<T> where T : class
{
    public float Total { get; set; }

    public List<T> Items { get; set; } = new();
}
