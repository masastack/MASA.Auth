// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid Id { get; set; }

    public string Department { get; set; } = "";

    public string Position { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public bool Enabled { get; set; }

    public StaffTypes StaffType { get; set; }

    public UserDto User { get; set; } = new();

    public StaffDto()
    {
    }

    public StaffDto(Guid id, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, UserDto user)
    {
        Id = id;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        User = user;
    }
}


