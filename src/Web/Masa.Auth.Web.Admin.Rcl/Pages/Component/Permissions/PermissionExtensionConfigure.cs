// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public class PermissionExtensionConfigure : PermissionsConfigure
{
    [Parameter]
    public List<UserPermissionDto> ExtensionValue { get; set; } = new();

    [Parameter]
    public EventCallback<List<UserPermissionDto>> ExtensionValueChanged { get; set; }

    protected override List<UniqueModel> ExpansionWrapperUniqueValue
    {
        get
        {
            var reject = ExtensionValue.Where(value => value.Effect is false)
                                       .Select(value => new UniqueModel(value.PermissionId.ToString()));

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
            //return ExtensionValue.Select(value => new UniqueModel(value.PermissionId.ToString()))
            //                    .Concat(RolePermissions.Select(value => new UniqueModel(value.ToString())))
            //                    .Except(reject)
            //                    .ToList();
        }
    }

    protected override async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
        var value = permissions.Select(permission => new UserPermissionDto(Guid.Parse(permission.Code), true)).ToList();
        foreach (var (code, parentCode) in EmptyPermissionMap)
        {
            if (value.Any(v => v.PermissionId == code)) value.Add(new(code, true));
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

    private async Task UpdateExtensionValueAsync(List<UserPermissionDto> value)
    {
        if (ExtensionValueChanged.HasDelegate) await ExtensionValueChanged.InvokeAsync(value);
        else ExtensionValue = value;
    }
}
