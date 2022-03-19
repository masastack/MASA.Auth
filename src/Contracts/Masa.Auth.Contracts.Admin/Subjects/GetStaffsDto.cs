namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetStaffsDto : Pagination
{
    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public bool Enabled { get; set; }

    public GetStaffsDto(int pageIndex, int pageSize, string name, string phoneNumber, string email, bool enabled)
    {
        Page = pageIndex;
        PageSize = pageSize;
        Name = name;
        PhoneNumber = phoneNumber;
        Email = email;
        Enabled = enabled;
    }

    public static ValueTask<GetStaffsDto?> BindAsync(HttpContext context, ParameterInfo parameter)
    {
        int.TryParse(context.Request.Query["page"], out var page);
        page = page == 0 ? 1 : page;
        int.TryParse(context.Request.Query["pageSize"], out var pageSize);
        pageSize = pageSize == 0 ? 20 : pageSize;
        bool.TryParse(context.Request.Query["enabled"], out var enabled);
        var name = context.Request.Query["name"];
        var phoneNumber = context.Request.Query["phoneNumber"];
        var email = context.Request.Query["email"];
        var result = new GetStaffsDto(page, pageSize, name, phoneNumber, email, enabled);

        return ValueTask.FromResult<GetStaffsDto?>(result);
    }
}

