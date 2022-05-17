// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyIdpService : RestServiceBase
{
    public ThirdPartyIdpService(IServiceCollection services) : base(services, "api/thirdPartyIdp")
    {
        MapPost(LdapSaveAsync, "ldap/save");
        MapPost(LdapConnectTestAsync, "ldap/connect-test");
    }

    #region ThirdPartyIdp

    private async Task<PaginationDto<ThirdPartyIdpDto>> GetListAsync(IEventBus eventBus, GetThirdPartyIdpsDto thirdPartyIdp)
    {
        var query = new ThirdPartyIdpsQuery(thirdPartyIdp.Page, thirdPartyIdp.PageSize, thirdPartyIdp.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ThirdPartyIdpDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ThirdPartyIdpDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ThirdPartyIdpSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string? search)
    {
        var query = new ThirdPartyIdpSelectQuery(search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(IEventBus eventBus, [FromBody] AddThirdPartyIdpDto dto)
    {
        await eventBus.PublishAsync(new AddThirdPartyIdpCommand(dto));
    }

    private async Task UpdateAsync(
        IEventBus eventBus,
        [FromBody] UpdateThirdPartyIdpDto dto)
    {
        await eventBus.PublishAsync(new UpdateThirdPartyIdpCommand(dto));
    }

    private async Task RemoveAsync(
        IEventBus eventBus,
        [FromBody] RemoveThirdPartyIdpDto dto)
    {
        await eventBus.PublishAsync(new RemoveThirdPartyIdpCommand(dto));
    }

    #endregion

    private async Task LdapSaveAsync(IEventBus eventBus, [FromBody] LdapDetailDto ldapDetailDto)
    {
        await eventBus.PublishAsync(new LdapUpsertCommand(ldapDetailDto));
    }

    private async Task LdapConnectTestAsync(IEventBus eventBus, [FromBody] LdapDetailDto ldapDetailDto)
    {
        await eventBus.PublishAsync(new LdapConnectTestCommand(ldapDetailDto));
    }
}
