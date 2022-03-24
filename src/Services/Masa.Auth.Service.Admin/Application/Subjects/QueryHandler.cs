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
    private async Task GetUserPaginationAsync(UserPaginationQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        condition.And(user => user.Enabled == query.Enabled);
        //Todo es search

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
    private async Task GetUserDetailAsync(UserDetailQuery query)
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

        query.Result = new(user.Id, user.Name, user.DisplayName, user.Avatar, user.IdCard, user.Account, user.CompanyName, user.Enabled, user.PhoneNumber, user.Email, user.CreationTime, user.Address, thirdPartyIdpAvatars, creator.Name, modifier.Name, user.ModificationTime, user.Department, user.Position, user.Password);
    }

    [EventHandler]
    private async Task<List<StaffDto>> StaffListAsync(StaffListQuery staffListQuery)
    {
        var key = staffListQuery.SearchKey;
        return (await _staffRepository.GetListAsync(s => s.JobNumber.Contains(key) || s.Name.Contains(key)))
            .Take(staffListQuery.MaxCount).Select(s => new StaffDto(s.Id, "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.Avatar, s.User.PhoneNumber, s.User.Email)).ToList();
    }

    [EventHandler]
    private async Task<PaginationDto<StaffDto>> StaffPaginationAsync(StaffPaginationQuery staffPaginationQuery)
    {
        var key = staffPaginationQuery.SearchKey;
        var page = staffPaginationQuery.Page;
        var pageSize = staffPaginationQuery.PageSize;
        Expression<Func<Staff, bool>> condition = staff => true;
        if (string.IsNullOrEmpty(key))
        {
            condition = condition.And(s => s.JobNumber.Contains(key) || s.Name.Contains(key));
        }
        var PaginationDto = await _staffRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = page,
            PageSize = pageSize
        });
        return new PaginationDto<StaffDto>(PaginationDto.Total, PaginationDto.TotalPages,
            PaginationDto.Result.Select(s => new StaffDto(s.Id, "", s.Position.Name, s.JobNumber, s.Enabled, s.User.Name, s.User.Avatar, s.User.PhoneNumber, s.User.Email)).ToList());
    }
}
