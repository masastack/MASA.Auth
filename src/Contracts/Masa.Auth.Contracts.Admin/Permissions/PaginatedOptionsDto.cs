// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class PaginatedOptionsDto
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public string Sorting { get; set; }

    public PaginatedOptionsDto(string sorting = "", int page = 1, int pageSize = 10)
    {
        Sorting = sorting;
        Page = page;
        PageSize = pageSize;
    }
}
