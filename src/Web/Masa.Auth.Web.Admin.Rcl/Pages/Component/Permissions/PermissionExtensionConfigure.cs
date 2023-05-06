﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public class PermissionExtensionConfigure : PermissionsConfigure
{
    [Parameter]
    public List<SubjectPermissionRelationDto> ExtensionValue { get; set; } = new();

    [Parameter]
    public EventCallback<List<SubjectPermissionRelationDto>> ExtensionValueChanged { get; set; }

    protected override List<UniqueModel> ExpansionWrapperUniqueValue
    {
        get
        {
            return ExtensionValue.Select(value =>
            {
                if (!value.Effect)
                {
                    return new UniqueModel(value.PermissionId.ToString(), false, true, false);
                }
                else return new UniqueModel(value.PermissionId.ToString());
            })
            .Union(RoleUnionTeamPermission.Select(value => new UniqueModel(value.ToString())))
            .ToList();
        }
    }

    protected override async Task RoleUnionTeamPermissionValueChangedAsync()
    {
        ExtensionValue = ExtensionValue.Where(e => e.Effect || RoleUnionTeamPermission.Contains(e.PermissionId)).ToList();

        // Remove parent menus that do not have child menu permissions
        foreach (var item in EmptyPermissionMap.IntersectBy(ExtensionValue.Select(e => e.PermissionId), p => p.Value))
        {
            var childPermissions = EmptyPermissionMap.Where(p => item.Value == p.Value).Select(p => p.Key).ToList();

            if (!childPermissions.Any(c => ExtensionValue.Any(p => p.PermissionId == c)))
            {
                ExtensionValue.First(e => e.PermissionId == item.Value).Effect = false;
            }
        }
        StateHasChanged();
    }

    protected override async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
        // Only the permissions set for the user are obtained. not contains role,team's permission
        var value = permissions.Select(permission => new SubjectPermissionRelationDto(Guid.Parse(permission.Code), true)).ToList();
        foreach (var permission in RoleUnionTeamPermission)
        {
            var rolePermissionValue = value.FirstOrDefault(v => v.PermissionId == permission);
            if (rolePermissionValue is null)
            {
                if (!EmptyPermissionMap.ContainsValue(permission))
                    value.Add(new(permission, false));
            }
            else
            {
                var existValue = ExtensionValue.FirstOrDefault(v => v.PermissionId == permission && v.Effect);
                if (existValue is null)
                {
                    value.Remove(rolePermissionValue);
                    ExtensionValue.Remove(rolePermissionValue);
                }
            }
        }
        value.AddRange(ExtensionValue.Where(ev => !ev.Effect && !value.Contains(ev)));

        // Filter not have child permission's parent
        var parentValue = new List<SubjectPermissionRelationDto>();
        foreach (var effectVal in value.IntersectBy(EmptyPermissionMap.Keys, p => p.PermissionId))
        {
            if (!EmptyPermissionMap.TryGetValue(effectVal.PermissionId, out Guid parentCode) || parentValue.Any(p => p.PermissionId == parentCode))
            {
                continue;
            }

            var childsCode = EmptyPermissionMap.Where(p => p.Value == parentCode).Select(p => p.Key).ToList();
            var childs = value.IntersectBy(childsCode, p => p.PermissionId).ToList();
            var roleUnionTeamPermissionIds = RoleUnionTeamPermission.Where(e => childsCode.Contains(e)).ToList();

            var notEffectChilds = childs.Where(e => e.Effect == false).Select(e => e.PermissionId).ToList();
            if (roleUnionTeamPermissionIds.Count > 0 && roleUnionTeamPermissionIds.All(p => notEffectChilds.Contains(p)))
            {
                parentValue.Add(new(parentCode, false));
            }
            else
            {
                parentValue.Add(new(parentCode, true));
            }
        }
        parentValue.ForEach(p => value.Insert(0, p));

        await UpdateExtensionValueAsync(value.Distinct().ToList());
    }

    private async Task UpdateExtensionValueAsync(List<SubjectPermissionRelationDto> value)
    {
        if (ExtensionValueChanged.HasDelegate)
        {
            await ExtensionValueChanged.InvokeAsync(value);
        }
        else
        {
            ExtensionValue = value;
        }
    }
}
