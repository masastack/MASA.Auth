namespace Masa.Auth.Service.Admin.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly ITeamRepository _teamRepository;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly AuthDbContext _authDbContext;
    readonly IEventBus _eventBus;
    readonly IAutoCompleteClient _autoCompleteClient;

    public QueryHandler(IUserRepository userRepository, ITeamRepository teamRepository, IStaffRepository staffRepository, IThirdPartyUserRepository thirdPartyUserRepository, AuthDbContext authDbContext, IEventBus eventBus, IAutoCompleteClient autoCompleteClient)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _authDbContext = authDbContext;
        _eventBus = eventBus;
        _autoCompleteClient = autoCompleteClient;
    }

    #region User

    [EventHandler]
    public async Task GetUsersAsync(UsersQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        if (query.Enabled is not null)
            condition = condition.And(user => user.Enabled == query.Enabled);

        if (query.StartTime is not null)
            condition = condition.And(user => user.CreationTime >= query.StartTime);

        if (query.EndTime is not null)
            condition = condition.And(user => user.CreationTime <= query.EndTime);

        if (query.UserId != Guid.Empty)
            condition = condition.And(user => user.Id == query.UserId);

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

        query.Result = new(users.Total, users.Result.Select(u => (UserDto)u).ToList());
    }

    [EventHandler]
    public async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.GetDetail(query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");
        var creator = await _authDbContext.Set<User>().Where(u => u.Id == user.Creator).Select(u => u.Name).FirstOrDefaultAsync();
        var modifier = await _authDbContext.Set<User>().Where(u => u.Id == user.Modifier).Select(u => u.Name).FirstOrDefaultAsync();

        query.Result = user;
        query.Result.Creator = creator ?? "";
        query.Result.Modifier = modifier ?? "";
    }

    [EventHandler]
    public async Task GetUserSelectAsync(UserSelectQuery query)
    {
        var response = await _autoCompleteClient.GetAsync<UserSelectDto, Guid>(query.Search);
        query.Result = response.Data;
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task GetStaffsAsync(StaffsQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (query.Enabled is not null)
            condition = condition.And(s => s.Enabled == query.Enabled);

        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));

        if (query.DepartmentId != Guid.Empty)
        {
            var staffIds = _authDbContext.Set<DepartmentStaff>()
                .Where(ds => ds.DepartmentId == query.DepartmentId)
                .Select(ds => ds.StaffId);
            condition = condition.And(s => staffIds.Contains(s.Id));
        }
        var staffQuery = _authDbContext.Set<Staff>().Where(condition);
        var total = await staffQuery.LongCountAsync();
        var staffs = await staffQuery.Include(s => s.User)
                                   .Include(s => s.DepartmentStaffs)
                                   .ThenInclude(ds => ds.Department)
                                   .Include(s => s.Position)
                                   .OrderByDescending(s => s.ModificationTime)
                                   .ThenByDescending(s => s.CreationTime)
                                   .Skip((query.Page - 1) * query.PageSize)
                                   .Take(query.PageSize)
                                   .ToListAsync();

        query.Result = new(total, staffs.Select(staff => (StaffDto)staff).ToList());
    }

    [EventHandler]
    public async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .Include(s => s.CreateUser)
                                        .Include(s => s.ModifyUser)
                                        .FirstOrDefaultAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException("This staff data does not exist");

        var userDetailQuery = new UserDetailQuery(staff.UserId);
        await _eventBus.PublishAsync(userDetailQuery);

        query.Result = staff;
        query.Result.User = userDetailQuery.Result;
    }

    [EventHandler]
    public async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));
        var staffs = await _staffRepository.GetPaginatedListAsync(condition, 0, query.MaxCount);

        query.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, "")).ToList();
    }

    #endregion

    #region ThirdPartyUser

    [EventHandler]
    public async Task GetThirdPartyUsersAsync(ThirdPartyUsersQuery query)
    {
        Expression<Func<ThirdPartyUser, bool>> condition = tpu => true;
        if (query.Enabled is not null)
            condition = condition.And(tpu => tpu.Enabled == query.Enabled);

        if (query.StartTime is not null)
            condition = condition.And(tpu => tpu.CreationTime >= query.StartTime);

        if (query.EndTime is not null)
            condition = condition.And(tpu => tpu.CreationTime <= query.EndTime);

        if (query.UserId != Guid.Empty)
            condition = condition.And(tpu => tpu.Id == query.UserId);

        var tpuQuery = _authDbContext.Set<ThirdPartyUser>().Where(condition);
        var total = await tpuQuery.LongCountAsync();
        var tpus = await tpuQuery.Include(tpu => tpu.CreateUser)
                                 .Include(tpu => tpu.ModifyUser)
                                 .OrderByDescending(s => s.ModificationTime)
                                 .ThenByDescending(s => s.CreationTime)
                                 .Skip((query.Page - 1) * query.PageSize)
                                 .Take(query.PageSize)
                                 .ToListAsync();

        query.Result = new(total, tpus.Select(tpu => (ThirdPartyUserDto)tpu).ToList());
    }

    [EventHandler]
    public async Task GetThirdPartyUserDetailAsync(ThirdPartyUserDetailQuery query)
    {
        var tpu = await _thirdPartyUserRepository.GetDetail(query.ThirdPartyUserId);
        if (tpu is null) throw new UserFriendlyException("This thirdPartyUser data does not exist");

        query.Result = tpu;
    }

    #endregion

    #region Team

    [EventHandler]
    public async Task TeamListAsync(TeamListQuery teamListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (!string.IsNullOrWhiteSpace(teamListQuery.Name))
        {
            condition = condition.And(s => s.Name.Contains(teamListQuery.Name));
        }
        teamListQuery.Result = (await _teamRepository.GetListAsync(condition))
                .Select(t => new TeamDto(t.Id, t.Name, t.Avatar.Url, t.Description, t.MemberCount, "", "", "", t.ModificationTime))
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
        if (!string.IsNullOrEmpty(teamSelectListQuery.Name))
        {
            condition = condition.And(s => s.Name.Contains(teamSelectListQuery.Name));
        }
        teamSelectListQuery.Result = (await _teamRepository.GetListAsync(condition))
                .Select(t => new TeamSelectDto(t.Id, t.Name, t.Avatar.Url))
                .ToList();
    }

    #endregion
}
