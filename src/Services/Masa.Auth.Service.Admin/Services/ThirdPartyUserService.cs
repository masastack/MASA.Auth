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
}
