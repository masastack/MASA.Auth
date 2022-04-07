namespace Masa.Auth.Service.Admin.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly ITeamRepository _teamRepository;
    readonly IStaffRepository _staffRepository;
    readonly AuthDbContext _authDbContext;
    readonly IEventBus _eventBus;

    public QueryHandler(IUserRepository userRepository, ITeamRepository teamRepository,
        IStaffRepository staffRepository, AuthDbContext authDbContext, IEventBus eventBus)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _authDbContext = authDbContext;
        _eventBus = eventBus;
    }

    #region User

    [EventHandler]
    public async Task GetUsersAsync(UsersQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        if (query.Enabled is not null)
            condition = condition.And(user => user.Enabled == query.Enabled);

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
        var creator = await _authDbContext.Set<User>().Select(u => new { Id = u.Id, Name = u.Name }).FirstOrDefaultAsync(u => u.Id == user.Creator);
        var modifier = await _authDbContext.Set<User>().Select(u => new { Id = u.Id, Name = u.Name }).FirstOrDefaultAsync(u => u.Id == user.Modifier);

        query.Result = user;
        query.Result.ThirdPartyIdpAvatars.AddRange(thirdPartyIdpAvatars);
        query.Result.Creator = creator?.Name ?? "";
        query.Result.Modifier = modifier?.Name ?? "";
    }

    [EventHandler]
    public async Task GetUserSelectAsync(UserSelectQuery query)
    {
        var user = await _userRepository.GetListAsync(u => u.Name == query.Search);
        //Todo es search
        query.Result = user.Select(u => new UserSelectDto(u.Id, u.Name, u.Account, u.PhoneNumber, u.Email, u.Avatar)).ToList();
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task GetStaffsAsync(GetStaffsQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (query.Enabled is not null)
            condition = condition.And(s => s.Enabled == query.Enabled);

        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(s => s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));

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
           new StaffDto(s.Id, s.DepartmentStaffs.FirstOrDefault()?.Department?.Name ?? "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.DisplayName, s.User.Avatar, s.User.PhoneNumber, s.User.Email)
       ).ToList());
    }

    [EventHandler]
    public async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs).FirstAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException("This staff data does not exist");

        var creator = await _authDbContext.Set<User>().Select(s => new { Id = s.Id, Name = s.Name }).FirstOrDefaultAsync(s => s.Id == staff.Creator);
        var modifier = await _authDbContext.Set<User>().Select(s => new { Id = s.Id, Name = s.Name }).FirstOrDefaultAsync(s => s.Id == staff.Modifier);
        var teams = staff.TeamStaffs.Select(ts => ts.TeamId).ToList();
        var department = staff.DepartmentStaffs.FirstOrDefault()?.DepartmentId ?? default;
        var userDetailQuery = new UserDetailQuery(staff.UserId);
        await _eventBus.PublishAsync(userDetailQuery);

        query.Result = new(staff.Id, department, staff.PositionId, staff.JobNumber, staff.Enabled, staff.StaffType, teams, userDetailQuery.Result, creator?.Name ?? "", modifier?.Name ?? "", staff.CreationTime, staff.ModificationTime);
    }

    [EventHandler]
    public async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        var staff = await _staffRepository.GetListAsync(u => u.JobNumber == query.Search);

        query.Result = staff.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, "")).ToList();
    }

    #endregion

    #region Team

    [EventHandler]
    public async Task TeamListAsync(TeamListQuery teamListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (string.IsNullOrEmpty(teamListQuery.Name))
        {
            condition = condition.And(s => s.Name.Contains(teamListQuery.Name));
        }
        teamListQuery.Result = (await _teamRepository.GetListAsync(condition))
                .Select(t => new TeamDto(t.Id, t.Name, t.Avatar.Url, t.Description, "", "", "", t.ModificationTime))
                .ToList();
    }

    [EventHandler]
    public async Task TeamDetailAsync(TeamDetailQuery teamDetailQuery)
    {
        var team = await _teamRepository.GetByIdAsync(teamDetailQuery.TeamId);
        teamDetailQuery.Result = new TeamDetailDto
        {
            Id = team.Id,
            TeamBasicInfo = new TeamBasicInfoDto
            {
                Name = team.Name,
                Description = team.Description,
                Type = (int)team.TeamType,
                Avatar = new AvatarValueDto
                {
                    Url = team.Avatar.Url,
                    Name = team.Avatar.Name,
                    Color = team.Avatar.Color
                }
            },
            TeamAdmin = new TeamPersonnelDto
            {

            },
            TeamMember = new TeamPersonnelDto
            {

            }
        };
    }

    [EventHandler]
    public async Task TeamSelectListAsync(TeamSelectListQuery teamSelectListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (string.IsNullOrEmpty(teamSelectListQuery.Name))
        {
            condition = condition.And(s => s.Name.Contains(teamSelectListQuery.Name));
        }
        teamSelectListQuery.Result = (await _teamRepository.GetListAsync(condition))
                .Select(t => new TeamSelectDto(t.Id, t.Name, t.Avatar.Url))
                .ToList();
    }

    #endregion
}
