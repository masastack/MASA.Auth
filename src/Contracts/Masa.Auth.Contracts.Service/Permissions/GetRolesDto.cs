// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class GetRolesDto : Pagination<GetRolesDto>
{
    public string Search { get; set; }

    public bool? Enabled { get; set; }

    public GetRolesDto(int page, int pageSize, string search, bool? enabled)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
    }
}

