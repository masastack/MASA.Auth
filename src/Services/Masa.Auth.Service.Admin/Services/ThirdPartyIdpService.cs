// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyIdpService : RestServiceBase
{
    public ThirdPartyIdpService() : base("api/thirdPartyIdp")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapPost(LdapSaveAsync, "ldap/save").RequireAuthorization();
        MapPost(LdapConnectTestAsync, "ldap/connect-test").RequireAuthorization();
        MapGet(LdapDetailAsync, "ldap/detail");
        MapGet(ldapOptionsAsync);
        MapGet(GetUserClaims);
    }

    #region ThirdPartyIdp

    private async Task<PaginationDto<ThirdPartyIdpDto>> GetListAsync(IEventBus eventBus, GetThirdPartyIdpsDto thirdPartyIdp)
    {
        var query = new ThirdPartyIdpsQuery(thirdPartyIdp.Page, thirdPartyIdp.PageSize, thirdPartyIdp.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<List<ThirdPartyIdpModel>> GetAllAsync(IEventBus eventBus)
    {
        var query = new AllThirdPartyIdpQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ThirdPartyIdpDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ThirdPartyIdpDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ThirdPartyIdpSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string? search, [FromQuery] bool includeLdap)
    {
        var query = new ThirdPartyIdpSelectQuery(search, includeLdap);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ThirdPartyIdpModel>> GetExternalThirdPartyIdpsAsync([FromServices] IEventBus eventBus)
    {
        var query = new ExternalThirdPartyIdpsQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private Dictionary<string, string> GetUserClaims()
    {
        return UserClaims.Claims;
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

    private async Task<LdapDetailDto> LdapDetailAsync(IEventBus eventBus)
    {
        var ldapDetailQuery = new LdapDetailQuery();
        await eventBus.PublishAsync(ldapDetailQuery);
        return ldapDetailQuery.Result;
    }

    private async Task<LdapOptionsModel?> ldapOptionsAsync(IEventBus eventBus, [FromQuery] string scheme)
    {
        var ldapDetailQuery = new LdapDetailQuery(scheme);
        await eventBus.PublishAsync(ldapDetailQuery);
        var ldap = ldapDetailQuery.Result;
        var options = new LdapOptionsModel(ldap.ServerAddress, ldap.ServerPort, ldap.BaseDn, ldap.UserSearchBaseDn, ldap.GroupSearchBaseDn, ldap.RootUserDn, ldap.RootUserPassword);
        options.ServerPortSsl = Convert.ToInt32(ldapDetailQuery.Result.IsLdaps);
        return options;
    }
}
