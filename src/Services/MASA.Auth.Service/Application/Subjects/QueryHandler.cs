namespace MASA.Auth.Service.Application.Subjects;

public class QueryHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IStaffRepository _userRepository;

    public QueryHandler(IStaffRepository staffRepository, IStaffRepository userRepository)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
    }

    [EventHandler]
    private async Task<List<StaffItem>> StaffListAsync(StaffListQuery staffListQuery)
    {
        var key = staffListQuery.SearchKey;
        return (await _staffRepository.GetListAsync(s => s.JobNumber.Contains(key) || s.Name.Contains(key)))
            .Take(staffListQuery.MaxCount).Select(s => new StaffItem
            {
                Name = s.Name,
                JobNumber = s.JobNumber,
                Id = s.Id,
                Avatar = s.User.Avatar
            }).ToList();
    }

    [EventHandler]
    private async Task<PaginationList<StaffItem>> StaffPaginationAsync(StaffPaginationQuery staffPaginationQuery)
    {
        var key = staffPaginationQuery.SearchKey;
        var pageIndex = staffPaginationQuery.PageIndex;
        var pageSize = staffPaginationQuery.PageSize;
        var paginationList = await _staffRepository.GetPaginatedListAsync(s => s.JobNumber.Contains(key) || s.Name.Contains(key), pageIndex, pageSize, null);
        return new PaginationList<StaffItem>
        {
            Total = paginationList.Count,
            Items = paginationList.Select(s => new StaffItem
            {
                Name = s.Name,
                JobNumber = s.JobNumber,
                Id = s.Id,
                Avatar = s.User.Avatar
            }).ToList()
        };
    }

}
