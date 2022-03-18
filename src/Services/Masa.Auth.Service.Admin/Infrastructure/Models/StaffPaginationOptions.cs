namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class StaffPaginationOptions : PaginationOptions
{
    public string Name { get; set; } = string.Empty;

    //todo source generate
    public static ValueTask<StaffPaginationOptions?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        var result = new StaffPaginationOptions
        {
            Page = page,
            PageSize = pageSize,
            Name = context.Request.Query["name"],
        };

        return ValueTask.FromResult<StaffPaginationOptions?>(result);
    }
}
