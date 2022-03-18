namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetTeamsDto : Pagination
{
    public GetTeamsDto(int pageIndex, int pageSize)
    {
        Page = pageIndex;
        PageSize = pageSize;
    }
}

