// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserSystemDataDto
{
    public Guid UserId { get; set; }

    public string SystemId { get; set; } = string.Empty;

    public string Data { get; set; } = string.Empty;
}
