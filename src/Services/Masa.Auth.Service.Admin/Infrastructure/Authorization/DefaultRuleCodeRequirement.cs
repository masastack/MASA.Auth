// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultRuleCodeRequirement : IAuthorizationRequirement
{
    public string AppId { get; }

    public DefaultRuleCodeRequirement(string appId)
    {
        AppId = appId;
    }
}
