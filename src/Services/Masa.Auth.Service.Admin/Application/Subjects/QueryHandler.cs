namespace Masa.Auth.Service.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;

    public QueryHandler(IUserRepository userRepository, IStaffRepository staffRepository)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
    }

    [EventHandler]
    private async Task GetUserPaginationAsync(UserPaginationQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        condition.And(user => user.Enabled == query.Enabled);
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(user => user.Name.Contains(query.Search) || user.DisplayName.Contains(query.Search) || user.PhoneNumber.Contains(query.Search));
        
        var users = await _userRepository.GetPaginatedListAsync(condition,new PaginatedOptions 
        {
            Page = query.PageIndex,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        query.Result = new(users.Total,users.TotalPages,users.Result.Select(u => new UserItem 
        { 

        }));
    }

    [EventHandler]
    private async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Id == query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");

        query.Result = new();
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
    private async Task<PaginationItems<StaffItem>> StaffPaginationAsync(StaffPaginationQuery staffPaginationQuery)
    {
        var key = staffPaginationQuery.SearchKey;
        var pageIndex = staffPaginationQuery.PageIndex;
        var pageSize = staffPaginationQuery.PageSize;
        var paginationList = await _staffRepository.GetPaginatedListAsync(s => s.JobNumber.Contains(key) || s.Name.Contains(key), pageIndex, pageSize, null);
        return new PaginationItems<StaffItem>(paginationList.Count(), 0, paginationList.Select(s => new StaffItem
        {
            Name = s.Name,
            JobNumber = s.JobNumber,
            Id = s.Id,
            Avatar = s.User.Avatar
        }));
    }
}
