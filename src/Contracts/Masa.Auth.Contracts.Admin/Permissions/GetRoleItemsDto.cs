namespace Masa.Auth.Contracts.Admin.Permissions;

public class GetRoleItemsDto
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public string Search { get; set; }

    public bool Enabled { get; set; }

    public GetRoleItemsDto(int pageIndex, int pageSize, string search, bool enabled)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }
}

