// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public class PermissionExtensionConfigure : PermissionsConfigure
{
    [Parameter]
    public List<PermissionSubjectRelationDto> ExtensionValue { get; set; } = new();

    [Parameter]
    public EventCallback<List<PermissionSubjectRelationDto>> ExtensionValueChanged { get; set; }

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
            .Union(RolePermissions.Select(value => new UniqueModel(value.ToString())))
            .ToList();
        }
    }

    protected override async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
        var value = permissions.Select(permission => new PermissionSubjectRelationDto(Guid.Parse(permission.Code), true)).ToList();
        value = value.Where(v => EmptyPermissionMap.Values.Contains(v.PermissionId) is false).ToList();
        foreach (var (code, parentCode) in EmptyPermissionMap)
        {
            if (value.Any(v => v.PermissionId == code)) value.Add(new(parentCode, true));
        }
        foreach (var permission in RolePermissions)
        {
            var rolePermissionValue = value.FirstOrDefault(v => v.PermissionId == permission);
            if (rolePermissionValue is null)
            {
                value.Add(new(permission, false));
            }
            else
            {
                value.Remove(rolePermissionValue);
                ExtensionValue.Remove(rolePermissionValue);
            }
        }
        value.AddRange(ExtensionValue.Where(value => value.Effect is false));
        await UpdateExtensionValueAsync(value.Distinct().ToList());
    }

    private async Task UpdateExtensionValueAsync(List<PermissionSubjectRelationDto> value)
    {
        if (ExtensionValueChanged.HasDelegate) await ExtensionValueChanged.InvokeAsync(value);
        else ExtensionValue = value;
    }
}
