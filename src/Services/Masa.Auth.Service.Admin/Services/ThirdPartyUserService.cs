// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyUserService : RestServiceBase
{
    public ThirdPartyUserService() : base("api/thirdPartyUser")
    {
        MapGet(GetAsync, "");
        MapPost(RegisterAsync, "register");
        MapPost(LdapUsersAccountAsync, "ldapUsersAccount");
        MapPost(ThirdPartyUserFieldValueAsync, "thirdPartyUserFieldValue");
    }

    private async Task<PaginationDto<ThirdPartyUserDto>> GetListAsync(IEventBus eventBus, GetThirdPartyUsersDto tpu)
    {
        var query = new ThirdPartyUsersQuery(tpu.Page, tpu.PageSize, tpu.Search, tpu.ThirdPartyId, tpu.Enabled, tpu.StartTime, tpu.EndTime);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ThirdPartyUserDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ThirdPartyUserDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<UserModel?> GetAsync(IEventBus eventBus, [FromQuery] string thridPartyIdentity)
    {
        var query = new ThirdPartyUserQuery(thridPartyIdentity);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<UserModel?> GetByUserIdAsync(IEventBus eventBus, [FromQuery] string scheme, Guid userId)
    {
        var identityProviderQuery = new IdentityProviderBySchemeQuery(scheme);
        await eventBus.PublishAsync(identityProviderQuery);
        var identityProvider = identityProviderQuery.Result;

        var query = new ThirdPartyUserByUserIdQuery(userId, identityProvider.Id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<string> GetThridPartyIdentityAsync(IEventBus eventBus, [FromQuery] string scheme, Guid userId)
    {
        var identityProviderQuery = new IdentityProviderBySchemeQuery(scheme);
        await eventBus.PublishAsync(identityProviderQuery);
        var identityProvider = identityProviderQuery.Result;

        var query = new ThridPartyIdentityQuery(userId, identityProvider.Id);
        await eventBus.PublishAsync(query);
        return query.Result ?? string.Empty;
    }

    private async Task<UserModel> UpsertThirdPartyUserExternalAsync(IEventBus eventBus, UpsertThirdPartyUserModel model)
    {
        var query = new UpsertThirdPartyUserExternalCommand(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<UserModel> AddThirdPartyUserAsync(
        IEventBus eventBus,
        [FromQuery] bool whenExistReturn,
        [FromQuery] bool whenExisUpdateClaimData,
        AddThirdPartyUserModel model)
    {
        var query = new AddThirdPartyUserExternalCommand(model, whenExistReturn, whenExisUpdateClaimData);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<UserModel> RegisterAsync(IEventBus eventBus, RegisterThirdPartyUserModel model)
    {
        var query = new RegisterThirdPartyUserCommand(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync(IEventBus eventBus, [FromBody] RemoveThirdPartyUserDto dto)
    {
        await eventBus.PublishAsync(new RemoveThirdPartyUserByIdCommand(dto.Id));
    }

    private async Task RemoveByThridPartyIdentityAsync(IEventBus eventBus, [FromBody] RemoveThirdPartyUserByThridPartyIdentityDto dto)
    {
        await eventBus.PublishAsync(new RemoveThirdPartyUserByThridPartyIdentityCommand(dto.ThridPartyIdentity));
    }

    private async Task<Dictionary<Guid, string>> LdapUsersAccountAsync(IEventBus eventBus, [FromBody] GetLdapUsersAccountDto dto)
    {
        var query = new LdapUsersAccountQuery(dto.UserIds);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<Dictionary<Guid, string>> ThirdPartyUserFieldValueAsync(IEventBus eventBus, [FromBody] GetThirdPartyUserFieldValueDto dto)
    {
        var query = new ThirdPartyUserFieldValueQuery(dto.Scheme, dto.UserIds, dto.Field);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
