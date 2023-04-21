// Copyright (c) MASA Stack All rights reserved.
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
        ExtensionValue = ExtensionValue.Where(e => e.Effect || (e.Effect == false && RoleUnionTeamPermission.Contains(e.PermissionId))).ToList();
    }

    protected override async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
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
                var existValue = ExtensionValue.FirstOrDefault(v => v.PermissionId == permission && !v.Effect);
                if (existValue is null)
                {
                    value.Remove(rolePermissionValue);
                    ExtensionValue.Remove(rolePermissionValue);
                }
            }
        }
        value.AddRange(ExtensionValue.Where(ev => !ev.Effect && !value.Contains(ev)));

        var parentValue = new List<SubjectPermissionRelationDto>();
        foreach (var effectVal in value.IntersectBy(EmptyPermissionMap.Keys, p => p.PermissionId))
        {
            if (!EmptyPermissionMap.TryGetValue(effectVal.PermissionId, out Guid parentCode) || parentValue.Any(p => p.PermissionId == parentCode))
            {
                continue;
            }

            var childsCode = EmptyPermissionMap.Where(p => p.Value == parentCode).Select(p => p.Key).ToList();
            var childs = value.IntersectBy(childsCode, p => p.PermissionId).ToList();
            if (childs.Count == childsCode.Count && childs.All(child => !child.Effect))
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
