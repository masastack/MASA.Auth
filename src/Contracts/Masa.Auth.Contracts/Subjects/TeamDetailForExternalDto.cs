// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamDetailForExternalDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string Description { get; set; } = "";

    public TeamTypes TeamType { get; set; }

    public List<StaffDto> TeamAdmin { get; set; } = new();

    public List<StaffDto> TeamMember { get; set; } = new();

    public TeamDetailForExternalDto()
    {

    }

    public TeamDetailForExternalDto(Guid id, string name, string avatar, string description, TeamTypes teamType, List<StaffDto> teamAdmin, List<StaffDto> teamMember)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Description = description;
        TeamType = teamType;
        TeamAdmin = teamAdmin;
        TeamMember = teamMember;
    }
}

public class TeamPersonnelForExternalDto
{
    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string Account { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public TeamPersonnelForExternalDto()
    {

    }

    public TeamPersonnelForExternalDto(string name, string displayName, string avatar, string account, string phoneNumber, string email)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        Account = account;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
