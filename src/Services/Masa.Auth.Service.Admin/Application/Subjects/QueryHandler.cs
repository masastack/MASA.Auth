namespace Masa.Auth.Service.Admin.Application.Subjects;

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
        if (!string.IsNullOrEmpty(query.Name))
            condition = condition.And(user => user.Name.Contains(query.Name));
        if (!string.IsNullOrEmpty(query.PhoneNumber))
            condition = condition.And(user => user.PhoneNumber.Contains(query.PhoneNumber));
        if (!string.IsNullOrEmpty(query.Email))
            condition = condition.And(user => user.Email.Contains(query.Email));


        var users = await _userRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.PageIndex,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        //query.Result = new(users.Total, users.TotalPages, users.Result.Select(u => new UserDto
        //{

        //}));
    }

    [EventHandler]
    private async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Id == query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");

        //query.Result = new();
    }

    [EventHandler]
    private async Task<List<StaffDto>> StaffListAsync(StaffListQuery staffListQuery)
    {
        var key = staffListQuery.SearchKey;
        return (await _staffRepository.GetListAsync(s => s.JobNumber.Contains(key) || s.Name.Contains(key)))
            .Take(staffListQuery.MaxCount).Select(s => new StaffDto(s.Id, "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.Avatar)).ToList();
    }

    [EventHandler]
    private async Task<PaginationDto<StaffDto>> StaffPaginationAsync(StaffPaginationQuery staffPaginationQuery)
    {
        var key = staffPaginationQuery.SearchKey;
        var pageIndex = staffPaginationQuery.PageIndex;
        var pageSize = staffPaginationQuery.PageSize;
        Expression<Func<Staff, bool>> condition = staff => true;
        if (string.IsNullOrEmpty(key))
        {
            condition = condition.And(s => s.JobNumber.Contains(key) || s.Name.Contains(key));
        }
        var PaginationDto = await _staffRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = pageIndex,
            PageSize = pageSize
        });
        return new PaginationDto<StaffDto>(PaginationDto.Total, PaginationDto.TotalPages,
            PaginationDto.Result.Select(s => new StaffDto(s.Id, "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.Avatar)).ToList());
    }
}
