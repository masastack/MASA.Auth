// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModel;

public class CacheUser
{
    public string Name { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string IdCard { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public bool Enabled { get; set; } = true;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Landline { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Position { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public GenderTypes Gender { get; set; }

    public List<Guid> Roles { get; set; } = new();

    public List<UserPermissionDto> Permissions { get; set; } = new();
}
