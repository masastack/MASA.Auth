// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class IdentityProviderDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public IdentityProviderDetailDto()
    {

    }

    public IdentityProviderDetailDto(Guid id, string name, string displayName, string icon, bool enabled)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Icon = icon;
        Enabled = enabled;
    }
}
