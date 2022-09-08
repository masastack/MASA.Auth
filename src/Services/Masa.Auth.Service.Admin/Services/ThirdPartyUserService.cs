// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ThirdPartyUserService : RestServiceBase
{
    public ThirdPartyUserService(IServiceCollection services) : base(services, "api/thirdPartyUser")
    {

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

    private async Task<UserModel> UpsertThirdPartyUserExternalAsync(IEventBus eventBus, UpsertThirdPartyUserModel model)
    {
        var query = new UpsertThirdPartyUserExternalCommand(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [AllowAnonymous]
    private async Task<UserModel> AddThirdPartyUserAsync(
        IEventBus eventBus,
        AddThirdPartyUserModel model)
    {
        var query = new AddThirdPartyUserExternalCommand(model);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
