﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Jobs;

public class SyncUserArgs
{
    public string Environment { get; set; } = string.Empty;

    public Guid UserId { get; set; }
}
