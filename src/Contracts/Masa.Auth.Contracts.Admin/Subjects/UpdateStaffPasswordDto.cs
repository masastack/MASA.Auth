// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateStaffPasswordDto
{
    public Guid Id { get; set; }

    public string Password { get; set; } = "";

    public UpdateStaffPasswordDto()
    {
    }

    public UpdateStaffPasswordDto(Guid id, string password)
    {
        Id = id;
        Password = password;
    }
}
