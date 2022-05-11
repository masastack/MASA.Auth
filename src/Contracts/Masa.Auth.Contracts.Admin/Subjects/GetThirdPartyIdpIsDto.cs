// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyIdpIsDto : Pagination<GetThirdPartyIdpIsDto>
{
    public string Search { get; set; }

    public GetThirdPartyIdpIsDto(int page, int pageSize, string search)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
    }
}

