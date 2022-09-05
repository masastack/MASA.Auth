// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class CustomLoginService : RestServiceBase
    {
        public CustomLoginService(IServiceCollection services) : base(services, "api/sso/customLogin")
        {
        }

        private async Task<PaginationDto<CustomLoginDto>> GetListAsync(IEventBus eventBus, GetCustomLoginsDto customLogin)
        {
            var query = new CustomLoginsQuery(customLogin.Page, customLogin.PageSize, customLogin.Search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<CustomLoginDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] int id)
        {
            var query = new CustomLoginDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        public async Task<CustomLoginModel?> GetCustomLoginByClientIdAsync(
            [FromServices] IEventBus eventBus, 
            [FromQuery] string clientId)
        {
            var query = new CustomLoginByClientIdQuery(clientId);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddAsync(IEventBus eventBus, [FromBody] AddCustomLoginDto dto)
        {
            await eventBus.PublishAsync(new AddCustomLoginCommand(dto));
        }

        private async Task UpdateAsync(
            IEventBus eventBus,
            [FromBody] UpdateCustomLoginDto dto)
        {
            await eventBus.PublishAsync(new UpdateCustomLoginCommand(dto));
        }

        private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemoveCustomLoginDto dto)
        {
            await eventBus.PublishAsync(new RemoveCustomLoginCommand(dto));
        }
    }
}
