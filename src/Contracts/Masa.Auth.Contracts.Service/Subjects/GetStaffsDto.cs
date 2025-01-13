// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetStaffsDto : Pagination<GetStaffsDto>
{
    public string Search { get; set; }

    public bool? Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public GetStaffsDto(int page, int pageSize, string search, bool? enabled, Guid departmentId)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        Enabled = enabled;
        DepartmentId = departmentId;
    }

    public GetStaffsDto(int page, int pageSize, string search, Guid departmentId) : this(page, pageSize, search, true, departmentId)
    {
    }

    public GetStaffsDto(int page, int pageSize, string search, bool? enabled) : this(page, pageSize, search, enabled, Guid.Empty)
    {
    }
}

