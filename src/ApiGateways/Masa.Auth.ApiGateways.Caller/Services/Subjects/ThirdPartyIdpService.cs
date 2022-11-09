// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyIdpService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ThirdPartyIdpService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/thirdPartyIdp/";
    }

    #region ThirdPartyIdp

    public async Task<PaginationDto<ThirdPartyIdpDto>> GetListAsync(GetThirdPartyIdpsDto request)
    {
        return await SendAsync<GetThirdPartyIdpsDto, PaginationDto<ThirdPartyIdpDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<ThirdPartyIdpSelectDto>> GetSelectAsync(string? search = null, bool includeLdap = false)
    {
        return await SendAsync<object, List<ThirdPartyIdpSelectDto>>(nameof(GetSelectAsync), new { search, includeLdap });
    }

    public async Task<List<ThirdPartyIdpModel>> GetExternalThirdPartyIdpsAsync()
    {
        return await GetAsync<List<ThirdPartyIdpModel>>(nameof(GetExternalThirdPartyIdpsAsync));
    }

    public async Task<Dictionary<string, string>> GetUserClaims()
    {
        return await GetAsync<Dictionary<string, string>>(nameof(GetUserClaims));
    }

    public async Task<ThirdPartyIdpDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, ThirdPartyIdpDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddThirdPartyIdpDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task AddStandardThirdPartyIdpsAsync()
    {
        await SendAsync<object?>(nameof(AddStandardThirdPartyIdpsAsync), null);
    }

    public async Task UpdateAsync(UpdateThirdPartyIdpDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveThirdPartyIdpDto(id));
    }

    #endregion

    #region LDAP

    public async Task LdapConnectTestAsync(LdapDetailDto ldapDetailDto)
    {
        await PostAsync("ldap/connect-test", ldapDetailDto);
    }

    public async Task LdapUpsertAsync(LdapDetailDto ldapDetailDto)
    {
        await PostAsync("ldap/save", ldapDetailDto);
    }

    public async Task<LdapDetailDto> GetLdapDetailAsync()
    {
        return await GetAsync<LdapDetailDto>($"ldap/detail");
    }

    #endregion
}
