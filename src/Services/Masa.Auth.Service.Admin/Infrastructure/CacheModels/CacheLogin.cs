// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

public class CacheLogin
{
    public string Account { get; set; } = "";

    public int LoginErrorCount { get; set; }

    public DateTimeOffset FreezeTime { get; set; }
}
