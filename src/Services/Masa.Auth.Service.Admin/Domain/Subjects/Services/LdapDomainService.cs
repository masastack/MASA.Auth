// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class LdapDomainService : DomainService
{
    private readonly AuthDbContext _authDbContext;
    private readonly UserDomainService _userDomainService;
    private readonly ILogger<LdapDomainService> _logger;

    public LdapDomainService(
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        ILogger<LdapDomainService> logger)
    {
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
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

        var addUsers = unExistLdapUsers.Select(ldapUser => new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true)));
        await _userDomainService.AddRangeAsync(addUsers.ToList());

        thirdPartyUsers.ForEach(tpu =>
        {
            var ldapUser = existLdapUsers.FirstOrDefault(ldapUser => ldapUser.ObjectGuid == tpu.ThridPartyIdentity);
            if (ldapUser != null)
            {
                tpu.User.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());

                if (tpu.User.Staff == null)
                {
                    tpu.User.Bind(new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true));
                }
                else
                {
                    tpu.User.Staff.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);
                }
            }
        });
        await _userDomainService.UpdateRangeAsync(thirdPartyUsers.Select(tpu => tpu.User).ToList());
    }

    string GetRelativeId(string objectSid)
    {
        var parts = objectSid.Split('-');
        if (parts.Length < 3)
        {
            return "";
        }

        return parts[parts.Length - 1];
    }

    public async Task<string> UpsertLdapUserAsync(LdapUser ldapUser)
    {
        var ldap = await GetIdentityProviderAsync();
        var user = await _authDbContext.Set<ThirdPartyUser>().Where(tpu => tpu.ThirdPartyIdpId == ldap.Id && tpu.ThridPartyIdentity == ldapUser.ObjectGuid)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff).Select(tpu => tpu.User).FirstOrDefaultAsync();
        if (user != null)
        {
            user.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", "", new());
            if (user.Staff != null)
            {
                user.Staff!.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);
            }
            else
            {
                var staff = new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true);
                user.Bind(staff);
            }

            await _userDomainService.UpdateAsync(user);
            return user.Account;
        }
        else
        {
            await _userDomainService.AddAsync(new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true)));
            return ldapUser.SamAccountName;
        }
    }

    public async Task<IdentityProvider> GetIdentityProviderAsync()
    {
        var identityProviderQuery = new IdentityProviderByTypeQuery(ThirdPartyIdpTypes.Ldap);
        await EventBus.PublishAsync(identityProviderQuery);
        return identityProviderQuery.Result;
    }
}
