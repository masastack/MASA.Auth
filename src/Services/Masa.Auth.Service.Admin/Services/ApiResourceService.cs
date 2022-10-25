// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class ApiResourceService : RestServiceBase
    {
        public ApiResourceService() : base("api/sso/apiResource")
        {
        }

        private async Task<PaginationDto<ApiResourceDto>> GetListAsync(IEventBus eventBus, GetApiResourcesDto apiResource)
        {
            var query = new ApiResourcesQuery(apiResource.Page, apiResource.PageSize, apiResource.Search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<ApiResourceDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
        {
            var query = new ApiResourceDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<ApiResourceSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string? search)
        {
            var query = new ApiResourceSelectQuery(search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddAsync(IEventBus eventBus, [FromBody] AddApiResourceDto dto)
        {
            await eventBus.PublishAsync(new AddApiResourceCommand(dto));
        }

        private async Task UpdateAsync(
            IEventBus eventBus,
            [FromBody] UpdateApiResourceDto dto)
        {
            await eventBus.PublishAsync(new UpdateApiResourceCommand(dto));
        }

        private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemoveApiResourceDto dto)
        {
            await eventBus.PublishAsync(new RemoveApiResourceCommand(dto));
        }
    }
}
