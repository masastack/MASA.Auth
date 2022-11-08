// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public class PermissionExtensionConfigure : PermissionsConfigure
{
    [Parameter]
    public List<SubjectPermissionRelationDto> ExtensionValue { get; set; } = new();

    [Parameter]
    public EventCallback<List<SubjectPermissionRelationDto>> ExtensionValueChanged { get; set; }

    //keep Nullable discriminate null and empty
    [Parameter]
    public List<SubjectPermissionRelationDto>? ScopeItems { get; set; }

    [Parameter]
    public List<Guid>? ScopeRoleItems { get; set; }

    List<SubjectPermissionRelationDto> _scopeItems = new();
    List<Guid> _scopeRoleItems = new();

    protected override void OnParametersSet()
    {
        if (ScopeItems?.SequenceEqual(_scopeItems) == false)
        {
            _scopeItems = ScopeItems;
            FilterCategories();
        }
        if (ScopeRoleItems?.SequenceEqual(_scopeRoleItems) == false)
        {
            _scopeRoleItems = ScopeRoleItems;
            FilterCategories();
        }
    }

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
        value = value.Where(v => EmptyPermissionMap.Values.Contains(v.PermissionId) is false).ToList();
        foreach (var (code, parentCode) in EmptyPermissionMap)
        {
            if (value.Any(v => v.PermissionId == code)) value.Add(new(parentCode, true));
        }
        foreach (var permission in RoleUnionTeamPermission)
        {
            var rolePermissionValue = value.FirstOrDefault(v => v.PermissionId == permission);
            if (rolePermissionValue is null)
            {
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
        value.AddRange(ExtensionValue.Where(value => value.Effect is false));
        await UpdateExtensionValueAsync(value.Distinct().ToList());
    }

    private async Task UpdateExtensionValueAsync(List<SubjectPermissionRelationDto> value)
    {
        if (ExtensionValueChanged.HasDelegate) await ExtensionValueChanged.InvokeAsync(value);
        else ExtensionValue = value;
    }

    void FilterCategories()
    {
        if (ScopeItems != null)
        {
            foreach (var category in _categories)
            {
                foreach (var app in category.Apps)
                {
                    app.Navs = FilterScopePermission(app.Navs);
                }                
            }
        }        

        List<Nav> FilterScopePermission(List<Nav> navs)
        {
            foreach (var nav in navs)
            {
                if (nav.Children.Any())
                {
                    nav.Children = FilterScopePermission(nav.Children);
                    nav.Hiden = nav.Children.All(c => c.Hiden);
                    continue;
                }

                if (ScopeItems.Where(p => !p.Effect).Select(p => p.PermissionId.ToString()).Contains(nav.Code))
                {
                    nav.IsDisabled = true;
                    nav.Hiden = false;
                    continue;
                }

                if (!ScopeItems.Where(p => p.Effect).Select(p => p.PermissionId.ToString()).Contains(nav.Code))
                {
                    nav.Hiden = true;
                    continue;
                }
                nav.Hiden = false;
            }
            return navs;
        }
    }

}
