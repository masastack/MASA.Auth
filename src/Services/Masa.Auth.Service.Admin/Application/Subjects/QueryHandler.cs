namespace Masa.Auth.Service.Admin.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly AuthDbContext _authDbContext;

    public QueryHandler(IUserRepository userRepository, IStaffRepository staffRepository, AuthDbContext authDbContext)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    private async Task GetUsersAsync(UsersQuery query)
    {
        Expression<Func<User, bool>> condition = user => user.Enabled == query.Enabled;
        if (query.userId != Guid.Empty)
            condition = condition.And(user => user.Id == query.userId);

        var users = await _userRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        query.Result = new(users.Total, users.TotalPages, users.Result.Select(u =>
            new UserDto(u.Id, u.Name, u.DisplayName, u.Avatar, u.IdCard, u.Account, u.CompanyName, u.Enabled, u.PhoneNumber, u.Email, u.CreationTime)
        ).ToList());
    }

    [EventHandler]
    public async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Id == query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");

        var thirdPartyIdpAvatars = await (from tpu in _authDbContext.Set<ThirdPartyUser>()
                                          where tpu.UserId == user.Id
                                          join tp in _authDbContext.Set<ThirdPartyIdp>()
                                          on tpu.ThirdPartyIdpId equals tp.Id
                                          into temp
                                          from tp in temp
                                          select tp.Icon).ToListAsync();
        var creator = await _authDbContext.Set<User>().Select(u => new { Id = u.Id, Name = u.Name }).FirstAsync(u => u.Id == user.Creator);
        var modifier = await _authDbContext.Set<User>().Select(u => new { Id = u.Id, Name = u.Name }).FirstAsync(u => u.Id == user.Modifier);

        query.Result = user;
        query.Result.ThirdPartyIdpAvatars.AddRange(thirdPartyIdpAvatars);
        query.Result.Creator = creator.Name;
        query.Result.Modifier = modifier.Name;
    }

    [EventHandler]
    private async Task GetUserSelectAsync(UserSelectQuery query)
    {
        var user = await _userRepository.GetListAsync(u => u.Name == query.Search);
        //Todo es search
        query.Result = user.Select(u => new UserSelectDto(u.Id, u.Name, u.Account, u.PhoneNumber, u.Email, u.Avatar)).ToList();
    }

    [EventHandler]
    private async Task GetStaffsAsync(GetStaffsQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => staff.Enabled == query.Enabled;
        if (query.StaffId != Guid.Empty)
        {
            condition = condition.And(s => s.Id == query.StaffId);
        }

        var staffs = await _staffRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Staff.ModificationTime)] = true,
                [nameof(Staff.CreationTime)] = true,
            }
        });

        query.Result = new(staffs.Total, staffs.TotalPages, staffs.Result.Select(s =>
           new StaffDto(s.Id, s.DepartmentStaff.Department.Name, s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.DisplayName,s.User.Avatar, s.User.PhoneNumber, s.User.Email)
       ).ToList());
    }

    [EventHandler]
    private async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _staffRepository.FindAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException("This staff data does not exist");

        var thirdPartyIdpAvatars = await (from tpu in _authDbContext.Set<ThirdPartyUser>()
                                          where tpu.UserId == staff.UserId
                                          join tp in _authDbContext.Set<ThirdPartyIdp>()
                                          on tpu.ThirdPartyIdpId equals tp.Id
                                          into temp
                                          from tp in temp
                                          select tp.Icon).ToListAsync();
        var creator = await _authDbContext.Set<Staff>().Select(s => new { Id = s.Id, Name = s.Name }).FirstAsync(s => s.Id == staff.Creator);
        var modifier = await _authDbContext.Set<Staff>().Select(s => new { Id = s.Id, Name = s.Name }).FirstAsync(s => s.Id == staff.Modifier);
        var teams = await _authDbContext.Set<TeamStaff>().Where(ts => ts.StaffId == query.StaffId).Select(ts => ts.TeamId).ToListAsync();

        query.Result = new(staff.Id, staff.DepartmentStaff.DepartmentId, staff.PositionId, staff.JobNumber, staff.Enabled, staff.StaffType, teams, staff.User, creator.Name, modifier.Name, staff.CreationTime, staff.ModificationTime);
    }

    [EventHandler]
    private async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        var staff = await _staffRepository.GetListAsync(u => u.JobNumber == query.Search);
        //Todo es search
        query.Result = staff.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, "")).ToList();
    }
}
