// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserPasswordDto
{
    public Guid Id { get; set; }

    public string Password { get; set; } = "";

    public UpdateUserPasswordDto()
    {
    }

    public UpdateUserPasswordDto(Guid id, string password)
    {
        Id = id;
        Password = password;
    }
}
