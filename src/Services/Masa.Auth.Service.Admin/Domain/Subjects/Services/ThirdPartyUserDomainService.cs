// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class ThirdPartyUserDomainService : DomainService
{
    public ThirdPartyUserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddThirdPartyUserAsync(AddThirdPartyUserDto thirdPartyUserDto)
    {
        await EventBus.PublishAsync(new AddThirdPartyUserDomainEvent(thirdPartyUserDto));
    }
}
