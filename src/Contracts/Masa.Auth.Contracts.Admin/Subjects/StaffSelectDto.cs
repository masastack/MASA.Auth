// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffSelectDto
{
    public Guid Id { get; set; }

    public string Account { get; set; }

    public string JobNumber { get; set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public StaffSelectDto(Guid id, string account, string jobNumber, string name, string displayName, string avatar)
    {
        Id = id;
        Account = account;
        JobNumber = jobNumber;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
    }
}


