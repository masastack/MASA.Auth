namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetTeamsDto : Pagination
{
    public GetTeamsDto(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}

