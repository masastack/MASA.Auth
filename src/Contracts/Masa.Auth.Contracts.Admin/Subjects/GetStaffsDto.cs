namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetStaffsDto : Pagination
{
    public Guid StaffId { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public GetStaffsDto(int page, int pageSize, string search, bool enabled, Guid departmentId)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
        DepartmentId = departmentId;
    }

    public GetStaffsDto(int page, int pageSize, string search, Guid departmentId) : this(page, pageSize, search, true, departmentId)
    {
    }

    public GetStaffsDto(int page, int pageSize, string search, bool enabled) : this(page, pageSize, search, enabled, Guid.Empty)
    {
    }

    public static ValueTask<GetStaffsDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        Guid.TryParse(context.Request.Query["departmentId"], out var departmentId);
        var search = context.Request.Query["search"];
        var result = new GetStaffsDto(page, pageSize, search, enabled, departmentId);

        return ValueTask.FromResult<GetStaffsDto?>(result);
    }
}

