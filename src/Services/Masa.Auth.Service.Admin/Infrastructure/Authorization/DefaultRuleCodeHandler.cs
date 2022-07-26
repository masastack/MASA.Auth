// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultRuleCodeHandler : AuthorizationHandler<DefaultRuleCodeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DefaultRuleCodeRequirement requirement)
    {
        //context.Fail();
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
