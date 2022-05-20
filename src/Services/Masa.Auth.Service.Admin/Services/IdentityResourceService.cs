// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services
{
    public class IdentityResourceService : RestServiceBase
    {
        public IdentityResourceService(IServiceCollection services) : base(services, "api/sso/identityResource")
        {
        }

        private async Task<PaginationDto<IdentityResourceDto>> GetListAsync(IEventBus eventBus, GetIdentityResourcesDto idrs)
        {
            var query = new IdentityResourcesQuery(idrs.Page, idrs.PageSize, idrs.Search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<IdentityResourceDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] int id)
        {
            var query = new IdentityResourceDetailQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task<List<IdentityResourceSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string search)
        {
            var query = new IdentityResourceSelectQuery(search);
            await eventBus.PublishAsync(query);
            return query.Result;
        }

        private async Task AddAsync(IEventBus eventBus, [FromBody] AddIdentityResourceDto dto)
        {
            await eventBus.PublishAsync(new AddIdentityResourceCommand(dto));
        }

        private async Task AddStandardIdentityResourcesAsync(IEventBus eventBus)
        {
            await eventBus.PublishAsync(new AddStandardIdentityResourcesCommand());
        }

        private async Task UpdateAsync(
            IEventBus eventBus,
            [FromBody] UpdateIdentityResourceDto dto)
        {
            await eventBus.PublishAsync(new UpdateIdentityResourceCommand(dto));
        }

        private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemoveIdentityResourceDto dto)
        {
            await eventBus.PublishAsync(new RemoveIdentityResourceCommand(dto));
        }
    }
}
