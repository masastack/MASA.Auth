namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyUsersDto : Pagination<GetThirdPartyUsersDto>
{
    public Guid UserId { get; set; }

    public bool? Enabled { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public GetThirdPartyUsersDto(int page, int pageSize, Guid userId, bool? enabled, DateTime? startTime, DateTime? endTime)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        Enabled = enabled;
        StartTime = startTime;
        EndTime = endTime;
    }
}

