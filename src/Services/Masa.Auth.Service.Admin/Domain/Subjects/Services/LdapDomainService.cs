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
        //Single-flight: a second batch sync triggered while one is still running would read the same
        //"no such user yet" snapshot and create duplicate users. Skip instead of queueing - the running
        //sync already covers the same data set.
        var executed = await SegmentedAsyncGate.TryRunBatchSyncAsync(() => SyncLdapUserCoreAsync(ldapUsers));
        if (!executed)
        {
            _logger.LogWarning("Another ldap user synchronization is still running, this batch is skipped.");
        }
    }

    async Task SyncLdapUserCoreAsync(IList<LdapUser> ldapUsers)
    {
        //Batch-internal deduplication. Since the filtered unique indexes (User.Account/Email/PhoneNumber
        //and Staff.JobNumber) are enforced, two ldap users in one batch conflicting on any of these keys
        //would make the whole AddRange/UpdateRange SaveChanges fail - keep the first occurrence instead.
        DeduplicateLdapUsers(ldapUsers);

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
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff)
            .Include(tpu => tpu.User).ThenInclude(user => user.Roles)
            .ToListAsync();

        //Index the batch by identity once: the add loop walks the (deduplicated) values skipping
        //identities already stored, and the update loop looks up the matching ldap user. GroupBy
        //instead of ToDictionary guards against ObjectGuid duplicates that DeduplicateLdapUsers
        //skips (null/empty keys are filtered out here as well).
        var ldapUsersByIdentity = ldapUsers
            .Where(ldapUser => !ldapUser.ObjectGuid.IsNullOrEmpty())
            .GroupBy(ldapUser => ldapUser.ObjectGuid)
            .ToDictionary(group => group.Key, group => group.First());
        var storedIdentities = thirdPartyUsers.Select(tpu => tpu.ThridPartyIdentity).ToHashSet();

        var positionCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var addUsers = new List<User>();
        foreach (var ldapUser in ldapUsersByIdentity.Values)
        {
            if (storedIdentities.Contains(ldapUser.ObjectGuid))
                continue;

            try
            {
                addUsers.Add(await BuildLdapUserAsync(ldap, ldapUser, positionCache));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to prepare Ldap user for creation, skipped.---- {Account} / {DisplayName}.", ldapUser.SamAccountName, ldapUser.DisplayName);
            }
        }
        await _userDomainService.AddRangeAsync(addUsers);

        //Only matched users are updated - previously every stored third-party user was rewritten
        //even when the batch contained no ldap user for it.
        var updatedUsers = new List<User>();
        foreach (var tpu in thirdPartyUsers)
        {
            if (!ldapUsersByIdentity.TryGetValue(tpu.ThridPartyIdentity, out var ldapUser))
                continue;
            updatedUsers.Add(tpu.User);
            try
            {
                tpu.User.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());

                if (tpu.User.Staff == null)
                {
                    var staff = new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company,
                        GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser, _logger),
                        await GetPositionIdAsync(ldapUser, positionCache), StaffTypes.Internal, !ldapUser.IsAccountDisabled);
                    tpu.User.Bind(staff);
                }
                else
                {
                    ApplyLdapProfile(tpu.User.Staff, ldapUser, await GetPositionIdAsync(ldapUser, positionCache));
                }

                //AD is the authoritative source for the account state: keep User.Enabled in sync
                //(not just Staff.Enabled), so a user disabled here is re-enabled once AD re-enables it.
                var ldapUserEnabled = !ldapUser.IsAccountDisabled;
                if (tpu.User.Enabled != ldapUserEnabled || (tpu.User.Staff != null && tpu.User.Staff.Enabled != ldapUserEnabled))
                {
                    tpu.User.SyncExternalIdpEnabled(ldapUserEnabled);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Ldap user, skipped.---- {Account} / {DisplayName}.", ldapUser.SamAccountName, ldapUser.DisplayName);
            }
        }
        await _userDomainService.UpdateRangeAsync(updatedUsers);
    }

    /// <summary>
    /// Batch-internal deduplication by the keys protected by filtered unique indexes:
    /// ObjectGuid, SamAccountName, EmailAddress, Phone and EmployeeNumber.
    /// Keep the first occurrence of each group and log the skipped accounts.
    /// </summary>
    void DeduplicateLdapUsers(IList<LdapUser> ldapUsers)
    {
        RemoveDuplicates(ldapUsers, user => user.ObjectGuid, nameof(LdapUser.ObjectGuid), StringComparer.Ordinal);
        RemoveDuplicates(ldapUsers, user => user.SamAccountName, nameof(LdapUser.SamAccountName), StringComparer.OrdinalIgnoreCase);
        RemoveDuplicates(ldapUsers, user => user.EmailAddress, nameof(LdapUser.EmailAddress), StringComparer.OrdinalIgnoreCase);
        RemoveDuplicates(ldapUsers, user => user.Phone, nameof(LdapUser.Phone), StringComparer.OrdinalIgnoreCase);
        RemoveDuplicates(ldapUsers, user => user.EmployeeNumber?.Trim() ?? "", nameof(LdapUser.EmployeeNumber), StringComparer.OrdinalIgnoreCase);
    }

    void RemoveDuplicates(IList<LdapUser> ldapUsers, Func<LdapUser, string> keySelector, string keyName, StringComparer comparer)
    {
        var removed = new List<LdapUser>();
        foreach (var group in ldapUsers.GroupBy(keySelector, comparer))
        {
            if (group.Key.IsNullOrEmpty() || group.Count() <= 1)
                continue;

            var skipped = group.Skip(1).ToList();
            removed.AddRange(skipped);
            _logger.LogWarning(
                "Duplicate ldap user {KeyName} '{Key}' found in the sync batch, keep {Keep} and skip: {Skipped}.",
                keyName, group.Key, group.First().SamAccountName, string.Join(',', skipped.Select(x => x.SamAccountName)));
        }
        ldapUsers.RemoveAll(removed);
    }

    /// <summary>
    /// JobNumber comes from AD attribute "employeeNumber"; empty when AD does not maintain it
    /// (no fallback — the SID RID previously used as fallback was wrong data).
    /// An over-length value is dropped with a warning: a truncated job number is still wrong data.
    /// </summary>
    internal static string GetEmployeeNumber(LdapUser ldapUser, ILogger logger)
    {
        var employeeNumber = ldapUser.EmployeeNumber?.Trim() ?? "";
        if (employeeNumber.Length > BusinessConsts.STAFF_JOB_NUMBER_MAX_LENGTH)
        {
            logger.LogWarning("Ldap employeeNumber '{employeeNumber}' exceeds JobNumber max length {MaxLength}, ignored.",
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
                //so detach it first. A unique index conflict (Position.Name) means a concurrent creation won -
                //reuse that position instead of leaving this user without one.
                _authDbContext.Entry(position).State = Microsoft.EntityFrameworkCore.EntityState.Detached;

                var existing = await _authDbContext.Set<Position>().FirstOrDefaultAsync(p => p.Name == employeeType);
                if (existing is not null)
                {
                    _logger.LogWarning(ex, "Creating Position '{PositionName}' lost the concurrency race, reuse the existing one.", employeeType);
                    positionCache[employeeType] = existing.Id;
                    return existing.Id;
                }

                _logger.LogError(ex, "Failed to create Position '{PositionName}' from Ldap employeeType, skipped.", employeeType);
                return null;
            }
        }

        positionCache[employeeType] = position.Id;
        return position.Id;
    }

    public async Task<string> UpsertLdapUserAsync(LdapUser ldapUser)
    {
        //Serialize concurrent "check-then-create" flows for the same AD account (e.g. the same user
        //login twice in the same second): the second request must see the user created by the first one.
        return await SegmentedAsyncGate.RunExclusiveAsync(ldapUser.ObjectGuid, () => UpsertLdapUserCoreAsync(ldapUser));
    }

    /// <summary>Must be called under the identity lock of ldapUser.ObjectGuid.</summary>
    async Task<string> UpsertLdapUserCoreAsync(LdapUser ldapUser)
    {
        var positionCache = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        var ldap = await GetIdentityProviderAsync();

        //Re-queried inside the lock: a concurrent request may have created the user already.
        var user = await FindLdapBoundUserAsync(ldap.Id, ldapUser.ObjectGuid);

        if (user is null && !ldapUser.Phone.IsNullOrEmpty())
        {
            user = await BindLdapUserByPhone(ldapUser, ldap.Id);
        }

        if (user != null)
        {
            await UpdateLdapBoundUserAsync(user, ldapUser, positionCache);
            return user.Account;
        }

        var newUser = await BuildLdapUserAsync(ldap, ldapUser, positionCache);
        try
        {
            await _userDomainService.AddAsync(newUser);
            return ldapUser.SamAccountName;
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is UserFriendlyException)
        {
            //A concurrent request (possibly from another instance) won the creation race: the filtered
            //unique index (or the re-check) rejected ours. Fall back to the existing user instead of
            //failing the whole login.
            var winner = await FindLdapBoundUserAsync(ldap.Id, ldapUser.ObjectGuid);
            if (winner is null)
                throw;

            _logger.LogWarning(ex, "Creation of ldap user {Account} lost the concurrency race, fallback to the existing user {UserId}.",
                ldapUser.SamAccountName, winner.Id);

            //No detach needed here: UserDomainService.AddAsync already detached the failed graph
            //before rethrowing (as DbUpdateException or the translated UserFriendlyException).
            await UpdateLdapBoundUserAsync(winner, ldapUser, positionCache);
            return winner.Account;
        }
    }

    async Task<User> BuildLdapUserAsync(IdentityProvider ldap, LdapUser ldapUser, Dictionary<string, Guid> positionCache)
    {
        var positionId = await GetPositionIdAsync(ldapUser, positionCache);
        var user = new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser, _logger), positionId, StaffTypes.Internal, !ldapUser.IsAccountDisabled));
        //An account disabled in AD must not be created as enabled.
        if (ldapUser.IsAccountDisabled)
        {
            user.Disable();
        }
        return user;
    }

    async Task UpdateLdapBoundUserAsync(User user, LdapUser ldapUser, Dictionary<string, Guid> positionCache)
    {
        user.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());
        if (user.Staff != null)
        {
            ApplyLdapProfile(user.Staff, ldapUser, await GetPositionIdAsync(ldapUser, positionCache));
        }
        else
        {
            var staff = new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetEmployeeNumber(ldapUser, _logger), await GetPositionIdAsync(ldapUser, positionCache), StaffTypes.Internal, !ldapUser.IsAccountDisabled);
            user.Bind(staff);
        }

        //AD is the authoritative source: an account re-enabled in AD must be usable again
        //(a sync may have disabled it earlier, which would otherwise block login forever).
        if (!user.Enabled && !ldapUser.IsAccountDisabled)
        {
            _logger.LogWarning("Ldap user was disabled in Auth but enabled in AD, re-enabled on login.---- {Account} / {DisplayName}.",
                ldapUser.SamAccountName, ldapUser.DisplayName);
            user.Enable();
        }

        await _userDomainService.UpdateAsync(user);
    }

    /// <summary>
    /// Apply the AD-sourced profile (name/display/gender/phone/email, job number, position) to an
    /// existing staff. Pure in-memory mutation - no Save, no webhook - shared by the batch sync and
    /// the single-user upsert. Account enable/disable is NOT part of it: the batch sync applies
    /// User.SyncExternalIdpEnabled (user + staff + roles/teams), while the login upsert
    /// only re-enables a user disabled in Auth but enabled in AD.
    /// </summary>
    void ApplyLdapProfile(Staff staff, LdapUser ldapUser, Guid? positionId)
    {
        staff.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);

        //JobNumber: AD is the authoritative source — always overwrite.
        //An empty employeeNumber clears legacy wrong values (e.g. the SID RID previously written).
        staff.UpdateJobNumber(GetEmployeeNumber(ldapUser, _logger));

        if (positionId != null)
        {
            staff.PositionId = positionId;
        }
    }

    Task<User?> FindLdapBoundUserAsync(Guid ldapId, string objectGuid)
        => _authDbContext.Set<ThirdPartyUser>()
            .Where(tpu => tpu.ThirdPartyIdpId == ldapId && tpu.ThridPartyIdentity == objectGuid)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff)
            .Select(tpu => tpu.User)
            .FirstOrDefaultAsync();

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
