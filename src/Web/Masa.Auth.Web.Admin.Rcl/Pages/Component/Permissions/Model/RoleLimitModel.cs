// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions.Model;

public class RoleLimitModel
{
    public string Role { get; set; } = "";

    public int Limit { get; set; }

    public RoleLimitModel(string role, int limit)
    {
        Role = role;
        Limit = limit;
    }
}
