// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class GetIdentityResourcesDto : Pagination
{
    public string Search { get; set; }

    public GetIdentityResourcesDto(int page, int pageSize, string search)
    {
        Search = search;
        Page = page;
        PageSize = pageSize;
    }
}

