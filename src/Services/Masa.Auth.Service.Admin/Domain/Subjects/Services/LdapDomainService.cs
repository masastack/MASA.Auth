// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class LdapDomainService : DomainService
{
    readonly AuthDbContext _authDbContext;
    readonly UserDomainService _userDomainService;

    public LdapDomainService(AuthDbContext authDbContext, UserDomainService userDomainService)
    {
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
    }

    public async Task SyncLdapUserAsync(IEnumerable<LdapUser> ldapUsers)
    {
        var ldap = await GetIdentityProviderAsync();

        var thirdPartyUsers = await _authDbContext.Set<ThirdPartyUser>().Where(tpu => tpu.ThirdPartyIdpId == ldap.Id)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff).ToListAsync();
        var existLdapUsers = ldapUsers.Where(ldapUser => thirdPartyUsers.Any(thirdPartyUser => thirdPartyUser.ThridPartyIdentity == ldapUser.ObjectGuid));
        var unExistLdapUsers = ldapUsers.ExceptBy(existLdapUsers.Select(user => user.ObjectGuid), user => user.ObjectGuid);

        var addUsers = unExistLdapUsers.Select(ldapUser => new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, true, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true)));
        await _userDomainService.AddRangeAsync(addUsers);

        thirdPartyUsers.ForEach(tpu =>
        {
            var ldapUser = existLdapUsers.FirstOrDefault(ldapUser => ldapUser.ObjectGuid == tpu.ThridPartyIdentity);
            if (ldapUser != null)
            {
                tpu.User.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", new());
                tpu.User.Staff!.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);
            }
        });
        await _userDomainService.UpdateRangeAsync(thirdPartyUsers.Select(tpu => tpu.User));
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

    public async Task UpsertLdapUserAsync(LdapUser ldapUser)
    {
        var ldap = await GetIdentityProviderAsync();
        var user = await _authDbContext.Set<ThirdPartyUser>().Where(tpu => tpu.ThirdPartyIdpId == ldap.Id && tpu.ThridPartyIdentity == ldapUser.ObjectGuid)
            .Include(tpu => tpu.User).ThenInclude(user => user.Staff).Select(tpu => tpu.User).FirstOrDefaultAsync();
        if (user != null)
        {
            user.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, "", "", "", new());
            user.Staff!.UpdateBasicInfo(ldapUser.Name, ldapUser.DisplayName, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress);
            await _userDomainService.UpdateAsync(user);
        }
        else
        {
            await _userDomainService.AddAsync(new User(ldapUser.Name, ldapUser.DisplayName, "", ldapUser.SamAccountName, "", ldapUser.Company, ldapUser.EmailAddress, ldapUser.Phone,
            new ThirdPartyUser(ldap.Id, true, ldapUser.ObjectGuid, JsonSerializer.Serialize(ldapUser)),
            new Staff(ldapUser.Name, ldapUser.DisplayName, "", "", ldapUser.Company, GenderTypes.Male, ldapUser.Phone, ldapUser.EmailAddress, GetRelativeId(ldapUser.ObjectSid), null, StaffTypes.Internal, true)));
        }
    }

    public async Task<IdentityProvider> GetIdentityProviderAsync()
    {
        var identityProviderQuery = new IdentityProviderByTypeQuery(ThirdPartyIdpTypes.Ldap);
        await EventBus.PublishAsync(identityProviderQuery);
        return identityProviderQuery.Result;
    }
}
