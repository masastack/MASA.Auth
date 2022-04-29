// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class GetClientPaginationDto : Pagination<GetClientPaginationDto>
{
    public string Search { get; set; } = string.Empty;
}
