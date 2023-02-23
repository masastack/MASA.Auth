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
                if (value.Effect is false)
                {
                    return new UniqueModel(value.PermissionId.ToString(), false, true, false);
                }
                else return new UniqueModel(value.PermissionId.ToString());
            })
            .Union(RoleUnionTeamPermission.Select(value => new UniqueModel(value.ToString())))
            .ToList();
        }
    }

    protected override async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
        var value = permissions.Select(permission => new SubjectPermissionRelationDto(Guid.Parse(permission.Code), true)).ToList();
        foreach (var permission in RoleUnionTeamPermission)
        {
            var rolePermissionValue = value.FirstOrDefault(v => v.PermissionId == permission);
            if (rolePermissionValue is null)
            {
                if(EmptyPermissionMap.ContainsValue(permission) is false)
                    value.Add(new(permission, false));
            }
            else
            {
                var existValue = ExtensionValue.FirstOrDefault(v => v.PermissionId == permission && v.Effect is true);
                if (existValue is null)
                {
                    value.Remove(rolePermissionValue);
                    ExtensionValue.Remove(rolePermissionValue);
                }
            }
        }
        value.AddRange(ExtensionValue.Where(ev => ev.Effect is false && value.Contains(ev) is false));
        foreach (var (code, parentCode) in EmptyPermissionMap)
        {
            if (value.Any(v => v.PermissionId == code)) value.Insert(0,new(parentCode, true));
        }
        await UpdateExtensionValueAsync(value.Distinct().ToList());
    }

    private async Task UpdateExtensionValueAsync(List<SubjectPermissionRelationDto> value)
    {
        if (ExtensionValueChanged.HasDelegate) await ExtensionValueChanged.InvokeAsync(value);
        else ExtensionValue = value;
    }
}
