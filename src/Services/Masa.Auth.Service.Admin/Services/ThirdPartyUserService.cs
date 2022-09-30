// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyUserService : RestServiceBase
{
    public ThirdPartyUserService() : base("api/thirdPartyUser")
    {
        MapGet(GetAsync, "");
    }

    private async Task<PaginationDto<ThirdPartyUserDto>> GetListAsync(IEventBus eventBus, GetThirdPartyUsersDto tpu)
    {
        var query = new ThirdPartyUsersQuery(tpu.Page, tpu.PageSize, tpu.UserId, tpu.Enabled, tpu.StartTime, tpu.EndTime);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<ThirdPartyUserDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new ThirdPartyUserDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<UserModel?> GetAsync(IEventBus eventBus, [FromQuery]string thridPartyIdentity )
    {
        var query = new ThirdPartyUserQuery(thridPartyIdentity);
        await eventBus.PublishAsync(query);
        return query.Result;
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
        AddThirdPartyUserModel model)
    {
        var query = new AddThirdPartyUserExternalCommand(model, whenExistReturn);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<UserModel> RegisterAsync(IEventBus eventBus, RegisterThirdPartyUserModel model)
    {
        var query = new RegisterThirdPartyUserCommand(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
