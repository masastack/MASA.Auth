// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyIdpsDto : Pagination
{
    public string Search { get; set; }

    public GetThirdPartyIdpsDto(int page, int pageSize, string search)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
    }
}

