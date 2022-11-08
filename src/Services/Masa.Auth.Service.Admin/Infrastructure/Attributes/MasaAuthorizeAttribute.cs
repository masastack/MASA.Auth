// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class MasaAuthorizeAttribute : AuthorizeAttribute, IMasaAuthorizeData
{
    public string Code { get; set; }

    public MasaAuthorizeAttribute()
    {
        Code = string.Empty;
    }

    public MasaAuthorizeAttribute(string code)
    {
        Code = code;
    }

    public MasaAuthorizeAttribute(params string[] roles)
    {
        Code = string.Empty;
        Roles = string.Join(',', roles);
    }
}
