namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetUsersDto : Pagination<GetUsersDto>
{
    public Guid UserId { get; set; }

    public bool? Enabled { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public GetUsersDto(int page, int pageSize, Guid userId, bool? enabled, DateTime? startTime, DateTime? endTime)
    {
        Page = page;
        PageSize = pageSize;
        UserId = userId;
        Enabled = enabled;
        StartTime = startTime;
        EndTime = endTime;
    }
}

