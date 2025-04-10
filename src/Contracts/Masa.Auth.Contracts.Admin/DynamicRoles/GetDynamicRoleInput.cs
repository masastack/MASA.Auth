// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class GetDynamicRoleInput : Pagination<GetDynamicRoleInput>
{
    public string Search { get; set; }

    public string ClientId { get; set; }

    public GetDynamicRoleInput(int page, int pageSize, string search, string clientId)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        ClientId = clientId;
    }
}