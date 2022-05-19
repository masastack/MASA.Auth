// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class SyncStaffResultsDto
{
    public List<SyncStaffResult> Results { get; set; } = new();

    public bool IsValid => Results.Count > 0;

    [DisallowNull]
    public SyncStaffResult? this[int index]
    {
        get => Results.FirstOrDefault(result => result.Index == index);
        set
        {
            value.Index = index;
            var result = Results.FirstOrDefault(result => result.Index == index);
            if (result is not null) result.Errors.AddRange(value.Errors);
            else Results.Add(value);
        }
    }

    public class SyncStaffResult
    {
        public int Index { get; set; }

        public string? Account { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}


