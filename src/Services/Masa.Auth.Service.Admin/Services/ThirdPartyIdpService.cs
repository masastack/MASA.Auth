// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyIdpService : ServiceBase
{
    public ThirdPartyIdpService(IServiceCollection services) : base(services, "api/thirdPartyIdp")
    {
        MapPost(LdapSaveAsync, "ldap/save");
        MapPost(LdapConnectTestAsync, "ldap/connect-test");
    }

    private async Task LdapSaveAsync(IEventBus eventBus, [FromBody] LdapDetailDto ldapDetailDto)
    {
        await eventBus.PublishAsync(new LdapUpsertCommand(ldapDetailDto));
    }

    private async Task LdapConnectTestAsync(IEventBus eventBus, [FromBody] LdapDetailDto ldapDetailDto)
    {
        await eventBus.PublishAsync(new LdapConnectTestCommand(ldapDetailDto));
    }
}
