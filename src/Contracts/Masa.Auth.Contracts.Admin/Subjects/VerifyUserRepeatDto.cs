// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class VerifyUserRepeatDto
{
    public Guid? Id { get; set; }

    public string? IdCard { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Landline { get; set; }

    public string? Email { get; set; }

    public string? Account { get; set; }
}
