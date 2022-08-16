// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpsertStaffForLdapDto
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public string Account { get; set; } = "";

    public string Password { get; set; } = "";

    public UpsertStaffForLdapDto()
    {
    }

    public UpsertStaffForLdapDto(bool enabled, string name, string displayName, string avatar, string phoneNumber, string email)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        PhoneNumber = phoneNumber;
        Email = email;
    }
}
