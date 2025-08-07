// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class StaffCommandHandler
{
    readonly IStaffRepository _staffRepository;
    readonly StaffDomainService _staffDomainService;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly ILogger<CommandHandler> _logger;
    readonly IEventBus _eventBus;
    readonly PhoneHelper _phoneHelper;
    readonly ILdapFactory _ldapFactory;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly LdapDomainService _ldapDomainService;
    readonly UserDomainService _userDomainService;
    readonly AuthDbContext _authDbContext;

    public StaffCommandHandler(
        IStaffRepository staffRepository,
        StaffDomainService staffDomainService,
        IDistributedCacheClient distributedCacheClient,
        ILogger<CommandHandler> logger,
        IEventBus eventBus,
        PhoneHelper phoneHelper,
        ILdapFactory ldapFactory,
        ILdapIdpRepository ldapIdpRepository,
        LdapDomainService ldapDomainService,
        UserDomainService userDomainService,
        AuthDbContext authDbContext)
    {
        _staffRepository = staffRepository;
        _staffDomainService = staffDomainService;
        _distributedCacheClient = distributedCacheClient;
        _logger = logger;
        _eventBus = eventBus;
        _phoneHelper = phoneHelper;
        _ldapFactory = ldapFactory;
        _ldapIdpRepository = ldapIdpRepository;
        _ldapDomainService = ldapDomainService;
        _userDomainService = userDomainService;
        _authDbContext = authDbContext;
    }

    [EventHandler(1)]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await _staffDomainService.AddAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        command.Result = await _staffDomainService.UpdateAsync(command.Staff);
    }

    [EventHandler(1)]
    public async Task ChangeStaffCurrentTeamAsync(UpdateStaffCurrentTeamCommand updateStaffCurrentTeamCommand)
    {
        var staff = await _staffRepository.FindAsync(s => s.UserId == updateStaffCurrentTeamCommand.UserId);
        if (staff == null)
        {
            _logger.LogError($"Can`t find staff by UserId = {updateStaffCurrentTeamCommand.UserId}");
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);
        }
        staff.SetCurrentTeam(updateStaffCurrentTeamCommand.TeamId);
        await _staffRepository.UpdateAsync(staff);
        updateStaffCurrentTeamCommand.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
    {
        var staffModel = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == command.Staff.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateBasicInfo(staffModel.Name, staffModel.DisplayName, staffModel.Gender, staffModel.PhoneNumber, staffModel.Email);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task UpdateStaffAvatarAsync(UpdateStaffAvatarCommand command)
    {
        var staffDto = command.Staff;
        var staff = await _staffRepository.FindAsync(s => s.UserId == staffDto.UserId);
        if (staff is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.STAFF_NOT_EXIST);

        staff.UpdateAvatar(staffDto.Avatar);
        await _staffRepository.UpdateAsync(staff);
        command.Result = staff;
    }

    [EventHandler(1)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        command.Result = await _staffDomainService.RemoveAsync(command.Staff.Id);
    }

    [EventHandler(1)]
    public async Task SyncStaffAsync(SyncStaffCommand command)
    {
        var syncResults = new SyncStaffResultsDto();
        command.Result = syncResults;
        var syncStaffs = command.Staffs;
        //validation
        var validator = new SyncStaffValidator(new PhoneNumberValidator(_phoneHelper));
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var staff = syncStaffs[i];
            var result = validator.Validate(staff);
            if (result.IsValid is false)
            {
                syncResults[i] = new()
                {
                    JobNumber = staff.JobNumber,
                    Errors = result.Errors.GroupBy(error => error.PropertyName).Select(e => e.First().ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        CheckDuplicate(Staff => Staff.PhoneNumber);
        CheckDuplicate(Staff => Staff.JobNumber);
        CheckDuplicate(Staff => Staff.Email);
        CheckDuplicate(Staff => Staff.IdCard);
        if (syncResults.IsValid) return;

        var defaultPasswordQuery = new StaffDefaultPasswordQuery();
        await _eventBus.PublishAsync(defaultPasswordQuery);
        string defaultPassword = defaultPasswordQuery.Result.DefaultPassword.WhenNullOrEmptyReplace(DefaultUserAttributes.Password);
        //sync user
        for (var i = 0; i < syncStaffs.Count; i++)
        {
            var syncStaff = syncStaffs[i];
            try
            {
                var (existStaff, _) = await _staffDomainService.VerifyRepeatAsync(syncStaff.JobNumber, syncStaff.PhoneNumber, syncStaff.Email, syncStaff.IdCard);
                if (existStaff is not null)
                {
                    var checkResult = await CheckSyncDataAsync(existStaff, syncStaff);
                    if (!checkResult.State)
                    {
                        if (checkResult.StaffResult != null)
                        {
                            syncResults[i] = checkResult.StaffResult;
                        }
                        continue;
                    }
                    await _staffDomainService.UpdateAsync(new UpdateStaffDto
                    {
                        Id = existStaff.Id,
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Email = syncStaff.Email,
                        IdCard = syncStaff.IdCard,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                        Position = syncStaff.Position,
                        PhoneNumber = syncStaff.PhoneNumber
                    });
                }
                else
                {
                    var addStaffDto = new AddStaffDto
                    {
                        Name = syncStaff.Name,
                        DisplayName = syncStaff.DisplayName,
                        Enabled = true,
                        Email = syncStaff.Email,
                        Password = defaultPassword,
                        PhoneNumber = syncStaff.PhoneNumber,
                        JobNumber = syncStaff.JobNumber,
                        IdCard = syncStaff.IdCard,
                        Position = syncStaff.Position,
                        Gender = syncStaff.Gender,
                        StaffType = syncStaff.StaffType,
                    };
                    await _staffDomainService.AddAsync(addStaffDto);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = ex is UserFriendlyException ? ex.Message : "Unknown exception, please contact the administrator";
                syncResults[i] = new()
                {
                    JobNumber = syncStaff.JobNumber,
                    Errors = new() { errorMsg }
                };
            }
        }

        void CheckDuplicate(Expression<Func<SyncStaffDto, string?>> selector)
        {
            var func = selector.Compile();
            if (syncStaffs.Where(staff => string.IsNullOrEmpty(func(staff)) is false).IsDuplicate(func, out List<SyncStaffDto>? duplicates))
            {
                foreach (var duplicate in duplicates)
                {
                    var index = syncStaffs.IndexOf(duplicate);
                    var staff = syncStaffs[index];
                    syncResults[index] = new()
                    {
                        JobNumber = staff.JobNumber,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }

        SyncStaffResultsDto.SyncStaffResult Error(string jobNumber, params string[] errorMessages) =>
            new SyncStaffResultsDto.SyncStaffResult()
            {
                JobNumber = jobNumber,
                Errors = errorMessages.ToList()
            };

        async Task<(bool State, SyncStaffResultsDto.SyncStaffResult? StaffResult)> CheckSyncDataAsync(Staff existStaff,
            SyncStaffDto syncStaff)
        {
            // Do not flush to db if the sync staff info is same with exist staff. 
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber &&
                existStaff.JobNumber == syncStaff.JobNumber &&
                existStaff.DisplayName == syncStaff.DisplayName.WhenNullOrEmptyReplace("") &&
                existStaff.Name == syncStaff.Name.WhenNullOrEmptyReplace("") &&
                existStaff.Gender == syncStaff.Gender &&
                existStaff.StaffType == syncStaff.StaffType &&
                existStaff.Position != null &&
                existStaff.Position.Name == syncStaff.Position.WhenNullOrEmptyReplace("") &&
                existStaff.Email == syncStaff.Email.WhenNullOrEmptyReplace("") &&
                existStaff.IdCard == syncStaff.IdCard.WhenNullOrEmptyReplace(""))
            {
                return (false, default);
            }

            // When phone number is not equals and job number is not equals, the email and id card cannot be equal! 
            if (existStaff.PhoneNumber != syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.Email) && existStaff.Email == syncStaff.Email)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard) && existStaff.IdCard == syncStaff.IdCard)
                {
                    return (false, Error(syncStaff.JobNumber,
                        $"The employee whose id card is {syncStaff.IdCard} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                }
            }

            // When job number is equals, the phone number must be equal
            if (existStaff.JobNumber == syncStaff.JobNumber && existStaff.PhoneNumber != syncStaff.PhoneNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose job number is {syncStaff.JobNumber}, the corresponding mobile phone number is {existStaff.PhoneNumber}, which does not match the mobile phone number {syncStaff.PhoneNumber}"));
            }

            // When phone number is equals, the job number must be equal
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber != syncStaff.JobNumber)
            {
                return (false, Error(syncStaff.JobNumber,
                    $"The employee whose mobile phone number is {syncStaff.PhoneNumber} has a corresponding job number of {existStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
            }

            // When job number is equal and phone number is equal, the staff whose is email or id card cannot be equal for other staff who is in database.
            if (existStaff.PhoneNumber == syncStaff.PhoneNumber && existStaff.JobNumber == syncStaff.JobNumber)
            {
                if (!string.IsNullOrWhiteSpace(syncStaff.IdCard))
                {
                    var (existIdCardStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, default, syncStaff.IdCard);
                    if (existIdCardStaff != null && existIdCardStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose id card number is {syncStaff.IdCard} has a corresponding job number of {existIdCardStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }

                if (!string.IsNullOrWhiteSpace(syncStaff.Email))
                {
                    var (existEmailStaff, _) = await _staffDomainService.VerifyRepeatAsync(default, default, syncStaff.Email, default);
                    if (existEmailStaff != null && existEmailStaff.JobNumber != syncStaff.JobNumber)
                    {
                        return (false, Error(syncStaff.JobNumber,
                            $"The employee whose email is {syncStaff.Email} has a corresponding job number of {existEmailStaff.JobNumber}, which does not match the job number of {syncStaff.JobNumber}"));
                    }
                }
            }

            return (true, default);
        }
    }

    [EventHandler(1)]
    public async Task UpdateStaffDefaultPasswordAsync(UpdateStaffDefaultPasswordCommand command)
    {
        await _distributedCacheClient.SetAsync(CacheKey.STAFF_DEFAULT_PASSWORD, command.DefaultPassword);
    }

    [EventHandler(1)]
    public async Task SyncStaffAsync(SyncFromLdapCommand command)
    {
        try
        {
            _logger.LogInformation("Starting LDAP staff synchronization...");

            // 1. 获取LDAP配置和提供程序
            var ldaps = await _ldapIdpRepository.GetListAsync();
            if (!ldaps.Any())
            {
                _logger.LogWarning("No LDAP configuration found, skipping synchronization");
                return;
            }

            var ldap = ldaps.First();
            var ldapOptions = ldap.Adapt<LdapOptions>();
            var ldapProvider = _ldapFactory.CreateProvider(ldapOptions);

            // 验证LDAP连接
            if (!await ldapProvider.AuthenticateAsync(ldapOptions.RootUserDn, ldapOptions.RootUserPassword))
            {
                _logger.LogError("Failed to authenticate with LDAP server");
                throw new UserFriendlyException("LDAP connection failed");
            }

            // 2. 获取所有LDAP用户（包含状态信息）
            var ldapUsers = new List<LdapUser>();
            await foreach (var user in ldapProvider.GetAllUserAsync())
            {
                ldapUsers.Add(user);
            }

            _logger.LogInformation($"Found {ldapUsers.Count} users in LDAP");

            // 3. 获取LDAP身份提供者
            var identityProvider = await _ldapDomainService.GetIdentityProviderAsync();

            // 4. 获取所有已存在的第三方用户（LDAP用户）
            var existingThirdPartyUsers = await _authDbContext.Set<ThirdPartyUser>()
                .Where(tpu => tpu.ThirdPartyIdpId == identityProvider.Id)
                .Include(tpu => tpu.User)
                .ThenInclude(user => user.Staff)
                .Include(tpu => tpu.User.Roles)
                .ToListAsync();

            _logger.LogInformation($"Found {existingThirdPartyUsers.Count} existing LDAP users in Auth system");

            // 5. 同步用户状态
            foreach (var tpu in existingThirdPartyUsers)
            {
                var ldapUser = ldapUsers.FirstOrDefault(lu => lu.ObjectGuid == tpu.ThridPartyIdentity);

                if (ldapUser != null)
                {
                    // LDAP中存在该用户，检查状态同步
                    var isLdapUserEnabled = IsLdapUserEnabled(ldapUser);
                    var isAuthUserEnabled = tpu.User.Enabled;

                    // 更新基本信息
                    var hasBasicInfoChanged = UpdateUserBasicInfo(tpu.User, ldapUser);

                    // 检查启用状态是否需要同步
                    if (isLdapUserEnabled != isAuthUserEnabled)
                    {
                        if (isLdapUserEnabled)
                        {
                            // LDAP用户已启用，Auth用户被禁用 -> 启用Auth用户
                            tpu.User.Enable();
                            if (tpu.User.Staff != null)
                            {
                                tpu.User.Staff.Enable();
                            }
                            _logger.LogInformation($"Enabled user {tpu.User.Account} based on LDAP status");
                        }
                        else
                        {
                            // LDAP用户已禁用，Auth用户被启用 -> 禁用Auth用户
                            tpu.User.Disable();
                            if (tpu.User.Staff != null)
                            {
                                tpu.User.Staff.Disable();
                                // 清除员工所在团队
                                tpu.User.Staff.SetTeamStaff(new List<Guid>());
                                tpu.User.Staff.SetCurrentTeam(Guid.Empty);
                            }

                            // 清除用户拥有的角色
                            if (tpu.User.Roles.Any())
                            {
                                var roleIds = tpu.User.Roles.Select(r => r.RoleId).ToList();
                                tpu.User.RemoveRoles(roleIds);
                            }

                            _logger.LogInformation($"Disabled user {tpu.User.Account} and cleared roles/teams based on LDAP status");
                        }
                    }

                    // 如果有任何变更，更新用户
                    if (hasBasicInfoChanged || isLdapUserEnabled != isAuthUserEnabled)
                    {
                        await _userDomainService.UpdateAsync(tpu.User);
                    }
                }
                //else
                //{
                //    // LDAP中不存在该用户，禁用Auth系统中的用户
                //    if (tpu.User.Enabled)
                //    {
                //        tpu.User.Disable();
                //        if (tpu.User.Staff != null)
                //        {
                //            tpu.User.Staff.Disable();
                //            tpu.User.Staff.SetTeamStaff(new List<Guid>());
                //            tpu.User.Staff.SetCurrentTeam(Guid.Empty);
                //        }

                //        // 清除角色
                //        if (tpu.User.Roles.Any())
                //        {
                //            var roleIds = tpu.User.Roles.Select(r => r.RoleId).ToList();
                //            tpu.User.RemoveRoles(roleIds);
                //        }

                //        await _userDomainService.UpdateAsync(tpu.User);
                //        syncStats = syncStats with { Disabled = syncStats.Disabled + 1, Updated = syncStats.Updated + 1 };
                //        _logger.LogWarning($"Disabled user {tpu.User.Account} - no longer exists in LDAP");
                //    }
                //}
            }

            _logger.LogInformation($"LDAP staff synchronization completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during LDAP staff synchronization");
            throw;
        }
    }

    /// <summary>
    /// 检查LDAP用户是否启用
    /// </summary>
    private bool IsLdapUserEnabled(LdapUser ldapUser)
    {
        return ldapUser.UserAccountControl == UserAccountControl.NormalAccount;
    }

    /// <summary>
    /// 更新用户对应的员工基本信息
    /// </summary>
    private bool UpdateUserBasicInfo(User user, LdapUser ldapUser)
    {
        var hasChanged = false;

        // 同步更新Staff信息
        if (user.Staff != null)
        {
            var changes = new List<string>();

            if (user.Staff.Name != ldapUser.Name)
            {
                changes.Add($"Name: '{user.Staff.Name}' -> '{ldapUser.Name}'");
            }
            if (user.Staff.DisplayName != ldapUser.DisplayName)
            {
                changes.Add($"DisplayName: '{user.Staff.DisplayName}' -> '{ldapUser.DisplayName}'");
            }
            if (user.Staff.Email != ldapUser.EmailAddress)
            {
                changes.Add($"Email: '{user.Staff.Email}' -> '{ldapUser.EmailAddress}'");
            }
            if (user.Staff.PhoneNumber != ldapUser.Phone)
            {
                changes.Add($"PhoneNumber: '{user.Staff.PhoneNumber}' -> '{ldapUser.Phone}'");
            }

            if (changes.Count > 0)
            {
                user.Staff.UpdateBasicInfo(
                    ldapUser.Name,
                    ldapUser.DisplayName,
                    user.Staff.Gender,
                    ldapUser.Phone,
                    ldapUser.EmailAddress);
                hasChanged = true;
                _logger.LogInformation($"Updated Staff basic info for user {user.Account} from LDAP. Changes: {string.Join("; ", changes)}");
            }
        }

        return hasChanged;
    }
}
