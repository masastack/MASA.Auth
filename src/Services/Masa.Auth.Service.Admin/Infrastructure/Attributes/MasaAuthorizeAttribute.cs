// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MasaAuthorizeAttribute : Attribute, IMasaAuthorizeData
{
    public string Code { get; set; }

    public string Account { get; set; }

    public MasaAuthorizeAttribute()
    {
        Code = string.Empty;
        Account = string.Empty;
    }

    public MasaAuthorizeAttribute(string code)
    {
        Code = code;
        Account = string.Empty;
    }
}
