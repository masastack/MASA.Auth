// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class PasswordRuleService : ServiceBase
{
    public PasswordRuleService() : base("api/password-rule")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapGet(GetGlobalPasswordRuleAsync);
    }

    private async Task<GlobalPasswordRuleDto> GetGlobalPasswordRuleAsync(IEventBus eventBus)
    {
        var query = new GlobalPasswordRuleQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
