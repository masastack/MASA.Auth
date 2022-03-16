﻿using Masa.Auth.Service.Admin.Application.Subjects.Models;
using Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

namespace Masa.Auth.Service.Admin.Application.Permissions.Models;

public class PermissionDetail
{
    public Guid Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public PermissionType Type { get; set; }

    public bool Enabled { get; set; }

    public string Description { get; set; } = "";

    public string AppId { get; set; } = "";

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public List<RoleSelectItem> RoleItems { get; set; } = new();

    public List<UserSelectItem> UserItems { get; set; } = new();

    public List<TeamSelectItem> TeamItems { get; set; } = new();
}