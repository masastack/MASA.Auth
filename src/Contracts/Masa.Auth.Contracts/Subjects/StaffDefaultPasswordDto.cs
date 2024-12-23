// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDefaultPasswordDto
{
    public bool Enabled { get; set; }

    public string DefaultPassword { get; set; } = "";
}
