namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetStaffsDto : Pagination
{
    public Guid StaffId { get; set; }

    public bool Enabled { get; set; }

    public GetStaffsDto(int page, int pageSize, Guid staffId, bool enabled)
    {
        Page = page;
        PageSize = pageSize;
        StaffId = staffId;
        Enabled = enabled;
    }

    public static ValueTask<GetStaffsDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        var staffId = Guid.Parse(context.Request.Query["staffId"]);
        var result = new GetStaffsDto(page, pageSize, staffId, enabled);

        return ValueTask.FromResult<GetStaffsDto?>(result);
    }
}

