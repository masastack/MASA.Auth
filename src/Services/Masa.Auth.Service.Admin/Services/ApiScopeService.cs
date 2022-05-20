// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class ApiScopeService : RestServiceBase
    {
        public ApiScopeService(IServiceCollection services) : base(services, "api/sso/apiScope")
        {
        }

        private async Task<PaginationDto<ApiScopeDto>> GetListAsync(IEventBus eventBus, GetApiScopesDto apiScope)
        {
            var query = new ApiScopesQuery(apiScope.Page, apiScope.PageSize, apiScope.Search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<ApiScopeDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] int id)
        {
            var query = new ApiScopeDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<ApiScopeSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string? search)
        {
            var query = new ApiScopeSelectQuery(search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddAsync(IEventBus eventBus, [FromBody] AddApiScopeDto dto)
        {
            await eventBus.PublishAsync(new AddApiScopeCommand(dto));
        }

        private async Task UpdateAsync(
            IEventBus eventBus,
            [FromBody] UpdateApiScopeDto dto)
        {
            await eventBus.PublishAsync(new UpdateApiScopeCommand(dto));
        }

        private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemoveApiScopeDto dto)
        {
            await eventBus.PublishAsync(new RemoveApiScopeCommand(dto));
        }
    }
}
