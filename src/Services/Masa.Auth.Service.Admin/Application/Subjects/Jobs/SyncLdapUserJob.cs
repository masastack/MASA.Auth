// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Jobs;

public class SyncLdapUserJob : BackgroundJobBase<SyncLdapUserArgs>
{
    private readonly LdapDomainService _ldapDomainService;

    public SyncLdapUserJob(LdapDomainService ldapDomainService)
    {
        _ldapDomainService = ldapDomainService;
    }

    protected override async Task ExecutingAsync(SyncLdapUserArgs args)
    {
        await _ldapDomainService.SyncLdapUserAsync(args.LdapUsers);
    }
}
