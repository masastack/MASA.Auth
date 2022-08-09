// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

public class CacheUserVisited
{
    public string AppId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;
}
