// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpsertStaffForLdapDto
{
    public Guid? UserId { get; set; }

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }

    public UpsertStaffForLdapDto()
    {
        DisplayName = "";
        PhoneNumber = "";
    }

    public UpsertStaffForLdapDto(Guid? userId, string? name, string displayName, string phoneNumber, string? email, string? account)
    {
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        PhoneNumber = phoneNumber;
        Email = email;
        Account = account;
    }
}
