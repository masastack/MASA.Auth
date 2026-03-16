// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class UpdateMenuMetaDto
{
    public Guid Id { get; set; }

    public string Icon { get; set; } = string.Empty;

    public string? MatchPattern { get; set; } = string.Empty;
}