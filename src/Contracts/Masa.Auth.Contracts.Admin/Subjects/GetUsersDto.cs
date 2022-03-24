namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetUsersDto : Pagination
{
    public string Search { get; set; }

    public bool Enabled { get; set; }

    public GetUsersDto(int page, int pageSize, string search, bool enabled)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }

    public static ValueTask<GetUsersDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {

        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        var search = context.Request.Query["search"];
        var result = new GetUsersDto(page, pageSize, search, enabled);

        return ValueTask.FromResult<GetUsersDto?>(result);
    }
}

