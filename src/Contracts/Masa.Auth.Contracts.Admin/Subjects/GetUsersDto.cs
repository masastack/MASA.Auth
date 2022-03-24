namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetUsersDto : Pagination
{
    public Guid UserId { get; set; }

    public bool Enabled { get; set; }

    public GetUsersDto(int page, int pageSize, Guid userId, bool enabled)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        Enabled = enabled;
    }

    public static ValueTask<GetUsersDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        var userId = Guid.Parse(context.Request.Query["userId"]);
        var result = new GetUsersDto(page, pageSize, userId, enabled);

        return ValueTask.FromResult<GetUsersDto?>(result);
    }
}

