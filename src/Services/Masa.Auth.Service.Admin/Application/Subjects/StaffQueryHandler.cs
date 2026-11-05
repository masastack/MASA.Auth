// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class StaffQueryHandler
{
    private readonly AuthDbContext _authDbContext;
    private readonly IStaffRepository _staffRepository;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private readonly OperaterProvider _operaterProvider;

    public StaffQueryHandler(
        AuthDbContext authDbContext,
        IStaffRepository staffRepository,
        IDistributedCacheClient distributedCacheClient,
        OperaterProvider operaterProvider)
    {
        _authDbContext = authDbContext;
        _staffRepository = staffRepository;
        _distributedCacheClient = distributedCacheClient;
        _operaterProvider = operaterProvider;
    }

    [EventHandler]
    public async Task GetStaffsAsync(StaffsQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (query.Enabled is not null)
            condition = condition.And(s => s.Enabled == query.Enabled);

        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(staff =>
                staff.DisplayName.Contains(query.Search) ||
                staff.PhoneNumber.Contains(query.Search) ||
                staff.JobNumber.Contains(query.Search) ||
                staff.Position!.Name.Contains(query.Search) ||
                staff.User.Account.Contains(query.Search));

        if (query.DepartmentId != Guid.Empty)
        {
            var staffIds = _authDbContext.Set<DepartmentStaff>()
                .Where(ds => ds.DepartmentId == query.DepartmentId)
                .Select(ds => ds.StaffId);
            condition = condition.And(s => staffIds.Contains(s.Id));
        }
        var staffQuery = _authDbContext.Set<Staff>()
                                       .Include(s => s.User)
                                       .Include(s => s.DepartmentStaffs)
                                       .ThenInclude(ds => ds.Department)
                                       .Include(s => s.Position)
                                       .Where(condition);
        var total = await staffQuery.LongCountAsync();
        var staffs = await staffQuery.AsNoTracking()
                                     .OrderByDescending(s => s.ModificationTime)
                                     .ThenByDescending(s => s.CreationTime)
                                     .Skip((query.Page - 1) * query.PageSize)
                                     .Take(query.PageSize)
                                     .ToListAsync();

        var staffDtos = staffs.Select(staff =>
        {
            var staffDto = (StaffDto)staff;
            staffDto.Account = staff.User.Account;
            return staffDto;
        }).ToList().ApplyDataMasking();

        query.Result = new(total, staffDtos);
    }

    [EventHandler]
    public async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.User)
                                        .Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        query.Result = staff;

        var (creator, modifier) = await _operaterProvider.GetActionInfoAsync(staff.Creator, staff.Modifier);
        query.Result.Creator = creator;
        query.Result.Modifier = modifier;
    }

    [EventHandler]
    public async Task GetStaffDetailByUserIdAsync(StaffDetailByUserIdQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.User)
                                        .ThenInclude(user => user.Roles)
                                        .ThenInclude(ur => ur.Role)
                                        .Include(s => s.DepartmentStaffs)
                                        .ThenInclude(ds => ds.Department)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .AsNoTracking()
                                        .AsSplitQuery()
                                        .FirstOrDefaultAsync(s => s.UserId == query.UserId);
        if (staff is not null)
        {
            var staffDetailModel = staff.Adapt<StaffDetailModel>();
            staffDetailModel.Department = staff.DepartmentStaffs.FirstOrDefault()?.Department?.Name ?? "";
            staffDetailModel.Position = staff.Position?.Name ?? "";
            staffDetailModel.Roles = staff.User.Roles.Select(ur => ur.Role).Adapt<List<RoleModel>>();
            staffDetailModel.Teams = new();
            query.Result = staffDetailModel;
        }
    }

    [EventHandler]
    public async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.DisplayName.Contains(query.Search) || s.Name.Contains(query.Search)
                || s.JobNumber.Contains(query.Search) || s.Email.Contains(query.Search)
                || s.PhoneNumber.Contains(query.Search));
        var staffs = await _staffRepository.GetListAsync(condition);

        query.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.DisplayName, s.Avatar, s.Email, s.PhoneNumber)).ToList();
    }

    [EventHandler]
    public async Task GetStaffSelectByIdsAsync(StaffSelectByIdQuery staffSelectByIdQuery)
    {
        var staffs = await _staffRepository.GetListAsync(s => staffSelectByIdQuery.Ids.Contains(s.Id));
        staffSelectByIdQuery.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.DisplayName, s.Avatar, s.Email, s.PhoneNumber)).ToList();
    }

    [EventHandler]
    public async Task GetStaffsByDepartmentAsync(StaffsByDepartmentQuery query)
    {
        var staffs = await _authDbContext.Set<Staff>()
                                         .Include(s => s.Position)
                                         .Include(staff => staff.DepartmentStaffs)
                                         .ThenInclude(departmentStaff => departmentStaff.Department)
                                         .Where(staff => staff.DepartmentStaffs.Any(department => department.DepartmentId == query.DepartmentId))
                                         .ToListAsync();

        query.Result = staffs.Select(staff => (StaffDto)staff).ToList().ApplyDataMasking();
    }

    [EventHandler]
    public async Task GetStaffsByTeamAsync(StaffsByTeamQuery query)
    {
        var staffs = await _authDbContext.Set<Staff>()
                                         .Include(s => s.Position)
                                         .Include(staff => staff.DepartmentStaffs)
                                         .ThenInclude(departmentStaff => departmentStaff.Department)
                                         .Include(staff => staff.TeamStaffs)
                                         .Where(staff => staff.TeamStaffs.Any(team => team.TeamId == query.TeamId))
                                         .ToListAsync();

        query.Result = staffs.Select(staff => (StaffDto)staff).ToList().ApplyDataMasking();
    }

    [EventHandler]
    public async Task GetUsersByRoleAsync(UsersByRoleQuery query)
    {
        var users = await _authDbContext.Set<User>()
                                         .Include(user => user.Roles)
                                         .Where(user => user.Roles.Any(role => role.RoleId == query.RoleId))
                                         .ToListAsync();

        query.Result = users?.Adapt<List<UserModel>>() ?? new();
    }

    [EventHandler]
    public async Task GetStaffTotalByDepartmentAsync(StaffTotalByDepartmentQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.DepartmentStaffs)
                                        .CountAsync(staff => staff.DepartmentStaffs.Any(department => department.DepartmentId == query.DepartmentId));

        query.Result = total;
    }

    [EventHandler]
    public async Task GetStaffTotalByTeamAsync(StaffTotalByTeamQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.TeamStaffs)
                                        .CountAsync(staff => staff.TeamStaffs.Any(team => team.TeamId == query.TeamId));

        query.Result = total;
    }

    [EventHandler]
    public async Task GetStaffTotalByRoleAsync(StaffTotalByRoleQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.User)
                                        .ThenInclude(user => user.Roles)
                                        .CountAsync(staff => staff.User.Roles.Any(role => role.RoleId == query.RoleId));

        query.Result = total;
    }

    [EventHandler]
    public async Task GetDefaultPasswordAsync(StaffDefaultPasswordQuery query)
    {
        var defaultPasswordDto = await _distributedCacheClient.GetAsync<StaffDefaultPasswordDto>(CacheKey.STAFF_DEFAULT_PASSWORD);
        query.Result = defaultPasswordDto ?? new();
    }
}
