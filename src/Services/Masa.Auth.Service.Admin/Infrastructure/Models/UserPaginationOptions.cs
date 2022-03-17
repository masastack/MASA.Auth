namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class UserPaginationOptions : PaginationOptions
{
    public string Search { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public static ValueTask<UserPaginationOptions?> BindAsync(HttpContext context, ParameterInfo parameter)
    {

        int.TryParse(context.Request.Query["pageIndex"], out var pageIndex);
        pageIndex = pageIndex == 0 ? 1 : pageIndex;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        var result = new UserPaginationOptions
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            Search = context.Request.Query["search"],
            Enabled = enabled
        };

        return ValueTask.FromResult<UserPaginationOptions?>(result);
    }
}