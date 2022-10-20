// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyUsersDto : Pagination<GetThirdPartyUsersDto>
{
    public string Search { get; set; }

    public Guid? ThirdPartyId { get; set; }

    public bool? Enabled { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public GetThirdPartyUsersDto(int page, int pageSize, string search, Guid? thirdPartyId, bool? enabled, DateTime? startTime, DateTime? endTime)
    {
        Page = page;
        PageSize = pageSize;
        Search = search;
        ThirdPartyId = thirdPartyId == Guid.Empty ? null : thirdPartyId;
        Enabled = enabled;
        StartTime = startTime;
        EndTime = endTime;
    }
}

