// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using StackApp = Masa.Stack.Components.Models.App;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class PermissionsCheck
{
    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public List<Guid> RoleIds { get; set; } = new();

    [Parameter]
    public Dictionary<Guid, bool> Value { get; set; } = new();

    [Parameter]
    public EventCallback<Dictionary<Guid, bool>> ValueChanged { get; set; }

    List<Category> _categories = new();
    List<CategoryAppNav> _initValue = new();
    List<CategoryAppNav> _allData = new();
    string _idPrefix = RandomUtils.GenerateSpecifiedString(8);

    ProjectService ProjectService => AuthCaller.ProjectService;

    RoleService RoleService => AuthCaller.RoleService;

    private async Task ValueChangedHandler(List<CategoryAppNav> _checkedItems)
    {
        if (_checkedItems != null)
        {
            var navKeys = _checkedItems.Where(i => !string.IsNullOrEmpty(i.Nav))
                .Select(i => i.Nav ?? "").Distinct().ToList();
            var valueKeys = Value.Keys.ToList();
            foreach (var valueKey in valueKeys)
            {
                Value[valueKey] = navKeys.Contains(valueKey.ToString());
            }
            foreach (var key in navKeys.ConvertAll(v => Guid.Parse(v)).Except(valueKeys))
            {
                Value[key] = true;
            }
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }
            else
            {
                _initValue = _checkedItems;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            if (RoleIds.Any())
            {
                await LoadRolePermissions();
            }
            InitChecked(Value.Select(v => v.Key.ToString()).ToList());
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadData()
    {
        var apps = (await ProjectService.GetListAsync(true)).SelectMany(p => p.Apps).ToList();
        _categories = apps.GroupBy(a => a.Tag).Select(ag => new Category
        {
            Code = ag.Key,
            Name = ag.Key,
            Apps = ag.Select(a => a.Adapt<StackApp>()).ToList()
        }).ToList();
        _allData.AddRange(_categories.SelectMany(category =>
            category.Apps.SelectMany(app => app.Navs.Select(nav =>
                new CategoryAppNav(category.Code, app.Code, nav.Code)))));
    }

    private async Task LoadRolePermissions()
    {
        var rolePermissions = await RoleService.GetPermissionsByRoleAsync(RoleIds);
        InitChecked(rolePermissions.Select(rp => rp.ToString()).ToList());
    }

    private void InitChecked(List<string> checkedItems)
    {
        _initValue.AddRange(_allData.Where(i => checkedItems.Contains(i.Nav ?? "")));
        _initValue = _initValue.Distinct().ToList();
    }
}
