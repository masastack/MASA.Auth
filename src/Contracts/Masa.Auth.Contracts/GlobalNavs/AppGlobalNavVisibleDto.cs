// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.GlobalNavs;

public class AppGlobalNavVisibleDto
{
    public string AppId { get; set; } = string.Empty;

    public GlobalNavVisibleTypes VisibleType { get; set; }

    public List<string> ClientIds { get; set; } = new();
}


public enum GlobalNavVisibleTypes
{
    AllVisible = 1,
    AllInvisible,
    Client,
}
