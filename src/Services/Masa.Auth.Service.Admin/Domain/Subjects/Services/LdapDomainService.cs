// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.
namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class LdapDomainService : DomainService
{
    private readonly AuthDbContext _authDbContext;
    private readonly UserDomainService _userDomainService;
    private readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    private readonly ILogger<LdapDomainService> _logger;

    public LdapDomainService(
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        IThirdPartyUserRepository thirdPartyUserRepository,
        ILogger<LdapDomainService> logger)
    {
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _logger = logger;
    }

    public async Task SyncLdapUserAsync(IList<LdapUser> ldapUsers)
    {
        //清除重复数据
        var duplicates = ldapUsers.GroupBy(x => x.Phone)
                       .Where(g => g.Count() > 1 && !g.Key.IsNullOrEmpty());

        if (duplicates.Any())
        {
            _logger.LogWarning("Duplicate phone numbers filtered.---- {data}.", string.Join(',', duplicates.Select(i => i.Key)));
            ldapUsers.RemoveAll(duplicates.SelectMany(g => g).ToList());
        }

        //清除手机号和邮箱同时为空的数据
        var illegalItems = ldapUsers.Where(ldapUser => ldapUser.EmailAddress.IsNullOrEmpty() && ldapUser.Phone.IsNullOrEmpty()).ToList();

        if (illegalItems.Count > 0)
        {
            _logger.LogWarning("There are {N} pieces of illegal data, both phone number and email are empty.---- {data}.",
                illegalItems.Count, string.Join(',', illegalItems.Select(i => i.DisplayName)));
            ldapUsers.RemoveAll(illegalItems);
        }

        var ldap = await GetIdentityProviderAsync();

        var thirdPartyUsers = await _authDbContext.Set<ThirdPartyUser>().Where(tpu => tpu.ThirdPartyIdpId == ldap.Id)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff).ToListAsync();
        var existLdapUsers = ldapUsers.Where(ldapUser => thirdPartyUsers.Any(thirdPartyUser => thirdPartyUser.ThridPartyIdentity == ldapUser.ObjectGuid));
        var unExistLdapUsers = ldapUsers.ExceptBy(existLdapUsers.Select(user => user.ObjectGuid), user => user.ObjectGuid).ToList();

        var positionCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var addUsers = new List<User>();
        foreach (var ldapUser in unExistLdapUsers)
        {
            try
            {
                var positionId = await GetPositionIdAsync(ldapUser, positionCache);
                addUsers.Add(new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
                    new ThirdPartyUser(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
                    new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser), positionId, StaffTypes.Internal, true)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to prepare Ldap user for creation, skipped.---- {Account} / {DisplayName}.", ldapUser.SamAccountName, ldapUser.DisplayName);
            }
        }
        await _userDomainService.AddRangeAsync(addUsers);

        foreach (var tpu in thirdPartyUsers)
        {
            var ldapUser = existLdapUsers.FirstOrDefault(ldapUser => ldapUser.ObjectGuid == tpu.ThridPartyIdentity);
            if (ldapUser != null)
            {
                try
                {
                    tpu.User.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());

                    if (tpu.User.Staff == null)
                    {
                        var staff = new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company,
                            GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser),
                            await GetPositionIdAsync(ldapUser, positionCache), StaffTypes.Internal, ldapUser.UserAccountControl == UserAccountControl.NormalAccount);
                        tpu.User.Bind(staff);
                    }
                    else
                    {
                        tpu.User.Staff.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);

                        //JobNumber: AD is the authoritative source — always overwrite.
                        //An empty employeeNumber clears legacy wrong values (e.g. the SID RID previously written).
                        tpu.User.Staff.UpdateJobNumber(GetEmployeeNumber(ldapUser));

                        var positionId = await GetPositionIdAsync(ldapUser, positionCache);
                        if (positionId != null)
                        {
                            tpu.User.Staff.PositionId = positionId;
                        }

                        if (ldapUser.UserAccountControl == UserAccountControl.NormalAccount)
                        {
                            tpu.User.Staff.Enable();
                        }
                        else
                        {
                            tpu.User.Staff.Disable();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to sync Ldap user, skipped.---- {Account} / {DisplayName}.", ldapUser.SamAccountName, ldapUser.DisplayName);
                }
            }
        }
        await _userDomainService.UpdateRangeAsync(thirdPartyUsers.Select(tpu => tpu.User).ToList());
    }

    /// <summary>
    /// JobNumber comes from AD attribute "employeeNumber"; empty when AD does not maintain it
    /// (no fallback — the SID RID previously used as fallback was wrong data).
    /// An over-length value is dropped with a warning: a truncated job number is still wrong data.
    /// </summary>
    string GetEmployeeNumber(LdapUser ldapUser)
    {
        var employeeNumber = ldapUser.EmployeeNumber?.Trim() ?? "";
        if (employeeNumber.Length > BusinessConsts.STAFF_JOB_NUMBER_MAX_LENGTH)
        {
            _logger.LogWarning("Ldap employeeNumber '{employeeNumber}' exceeds JobNumber max length {MaxLength}, ignored.",
                employeeNumber, BusinessConsts.STAFF_JOB_NUMBER_MAX_LENGTH);
            return "";
        }
        return employeeNumber;
    }

    /// <summary>
    /// Resolve AD attribute "employeeType" (job level/category) to a Position aggregate,
    /// matching by name and creating it when missing. Returns null when employeeType is empty.
    /// </summary>
    async Task<Guid?> GetPositionIdAsync(LdapUser ldapUser, Dictionary<string, Guid> positionCache)
    {
        var employeeType = ldapUser.EmployeeType?.Trim() ?? "";
        if (employeeType.IsNullOrEmpty())
        {
            return null;
        }

        if (employeeType.Length > BusinessConsts.POSITION_NAME_MAX_LENGTH)
        {
            _logger.LogWarning("Ldap employeeType '{employeeType}' exceeds Position.Name max length {MaxLength}, truncated.",
                employeeType, BusinessConsts.POSITION_NAME_MAX_LENGTH);
            employeeType = employeeType[..BusinessConsts.POSITION_NAME_MAX_LENGTH];
        }

        if (positionCache.TryGetValue(employeeType, out var cachedPositionId))
        {
            return cachedPositionId;
        }

        var position = await _authDbContext.Set<Position>().FirstOrDefaultAsync(p => p.Name == employeeType);
        if (position is null)
        {
            try
            {
                position = new Position(employeeType);
                await _authDbContext.Set<Position>().AddAsync(position);
                await _authDbContext.SaveChangesAsync();
                _logger.LogInformation("Position '{PositionName}' created from Ldap employeeType.", employeeType);
            }
            catch (Exception ex)
            {
                //A failed insert would leave the entity tracked and break every following user of this batch,
                //so detach it and keep the batch going (positionId stays null for this user).
                _authDbContext.Entry(position).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                _logger.LogError(ex, "Failed to create Position '{PositionName}' from Ldap employeeType, skipped.", employeeType);
                return null;
            }
        }

        positionCache[employeeType] = position.Id;
        return position.Id;
    }

    public async Task<string> UpsertLdapUserAsync(LdapUser ldapUser)
    {
        var positionCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var ldap = await GetIdentityProviderAsync();
        var user = await _authDbContext.Set<ThirdPartyUser>().Where(tpu => tpu.ThirdPartyIdpId == ldap.Id && tpu.ThridPartyIdentity == ldapUser.ObjectGuid)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff).Select(tpu => tpu.User).FirstOrDefaultAsync();

        if (user is null && !ldapUser.Phone.IsNullOrEmpty())
        {
            user = await BindLdapUserByPhone(ldapUser, ldap.Id);
        }

        if (user != null)
        {
            user.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());
            if (user.Staff != null)
            {
                user.Staff!.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);

                //JobNumber: AD is the authoritative source — always overwrite.
                //An empty employeeNumber clears legacy wrong values (e.g. the SID RID previously written).
                user.Staff!.UpdateJobNumber(GetEmployeeNumber(ldapUser));

                var positionId = await GetPositionIdAsync(ldapUser, positionCache);
                if (positionId != null)
                {
                    user.Staff!.PositionId = positionId;
                }
            }
            else
            {
                var staff = new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser), await GetPositionIdAsync(ldapUser, positionCache), StaffTypes.Internal, true);
                user.Bind(staff);
            }

            await _userDomainService.UpdateAsync(user);
            return user.Account;
        }
        else
        {
            await _userDomainService.AddAsync(new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser), await GetPositionIdAsync(ldapUser, positionCache), StaffTypes.Internal, true)));
            return ldapUser.SamAccountName;
        }
    }

    public async Task<IdentityProvider> GetIdentityProviderAsync()
    {
        var identityProviderQuery = new IdentityProviderByTypeQuery(ThirdPartyIdpTypes.Ldap);
        await EventBus.PublishAsync(identityProviderQuery);
        return identityProviderQuery.Result;
    }

    private async Task<User?> BindLdapUserByPhone(LdapUser ldapUser, Guid ldapId)
    {
        var user = await _authDbContext.Set<User>().Include(x => x.Staff).Include(x => x.ThirdPartyUsers).FirstOrDefaultAsync(x => x.PhoneNumber == ldapUser.Phone);
        if (user is not null && !user.ThirdPartyUsers.Any(x => x.ThirdPartyIdpId == ldapId))
        {
            var thirdPartyUser = new ThirdPartyUser(ldapId, user.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser));
            await _thirdPartyUserRepository.AddAsync(thirdPartyUser);
        }

        return user;
    }
}
