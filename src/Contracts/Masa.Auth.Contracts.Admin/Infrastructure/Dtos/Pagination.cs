namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class Pagination<T> : FromUri<T>
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}


