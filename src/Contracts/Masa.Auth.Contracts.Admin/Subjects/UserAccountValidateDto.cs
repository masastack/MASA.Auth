// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserAccountValidateDto
{
    public string Account { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
