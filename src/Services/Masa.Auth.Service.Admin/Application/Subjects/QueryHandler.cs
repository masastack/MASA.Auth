namespace Masa.Auth.Service.Admin.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly ITeamRepository _teamRepository;
    readonly IStaffRepository _staffRepository;
    readonly AuthDbContext _authDbContext;

    public QueryHandler(IUserRepository userRepository, ITeamRepository teamRepository,
        IStaffRepository staffRepository, AuthDbContext authDbContext)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    public async Task GetUserPaginationAsync(UserPaginationQuery query)
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
            Page = query.Page,
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
    public async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Id == query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");

        //query.Result = new();
    }

    [EventHandler]
    public async Task StaffListAsync(StaffListQuery staffListQuery)
    {
        var key = staffListQuery.SearchKey;

    }

    [EventHandler]
    public async Task StaffPaginationAsync(StaffPaginationQuery staffPaginationQuery)
    {
        var key = staffPaginationQuery.SearchKey;
        var page = staffPaginationQuery.Page;
        var pageSize = staffPaginationQuery.PageSize;
        Expression<Func<Staff, bool>> condition = _ => true;
        if (string.IsNullOrEmpty(key))
        {
            condition = condition.And(s => s.JobNumber.Contains(key) || s.Name.Contains(key));
        }
        if (staffPaginationQuery.DepartmentId != Guid.Empty)
        {
            var staffIds = _authDbContext.Set<DepartmentStaff>()
                .Where(ds => ds.DepartmentId == staffPaginationQuery.DepartmentId)
                .Select(ds => ds.StaffId);
            condition = condition.And(s => staffIds.Contains(s.Id));
        }
        var PaginationDto = await _staffRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = page,
            PageSize = pageSize
        });
        staffPaginationQuery.Result = new PaginationDto<StaffDto>(PaginationDto.Total, PaginationDto.TotalPages,
            PaginationDto.Result.Select(s => new StaffDto(s.Id, "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.DisplayName, s.User.Avatar, s.User.PhoneNumber, s.User.Email)).ToList());
    }

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
            TeamBaseInfo = new TeamBaseInfoDto
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
