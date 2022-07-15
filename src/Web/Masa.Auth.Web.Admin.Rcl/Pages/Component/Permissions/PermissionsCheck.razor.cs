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

    private List<Guid> InternalRoleIds { get; set; } = new();

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
            InitChecked(Value.Where(v => v.Value).Select(v => v.Key.ToString()).ToList());
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            //if (RoleIds.Any())
            //{
            //    await LoadRolePermissions();
            //}
            //InitChecked(Value.Where(v => v.Value).Select(v => v.Key.ToString()).ToList());
            //StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if(RoleIds.Count != InternalRoleIds.Count)
        {
            InternalRoleIds = RoleIds;
            await LoadRolePermissions();
            //InitChecked(Value.Where(v => v.Value).Select(v => v.Key.ToString()).ToList());
        }
    }

    private async Task LoadData()
    {
        var apps = (await ProjectService.GetListAsync(true)).SelectMany(p => p.Apps).ToList();
        _categories = apps.GroupBy(a => a.Tag).Select(ag => new Category
        {
            Code = ag.Key.Replace(" ", ""),
            Name = ag.Key,
            Apps = ag.Select(a => a.Adapt<StackApp>()).ToList()
        }).ToList();
        _allData = DataConversion(_categories);
    }

    private async Task LoadRolePermissions()
    {
        var rolePermissions = await RoleService.GetPermissionsByRoleAsync(RoleIds);
        InitChecked(rolePermissions.Select(rp => rp.ToString()).ToList());
    }

    private void InitChecked(List<string> checkedItems)
    {
        _initValue = _allData.Where(i => checkedItems.Contains(i.Nav ?? "")).ToList();
    }

    private List<CategoryAppNav> DataConversion(List<Category> catetories)
    {
        var result = new List<CategoryAppNav>();

        var levelData = catetories.SelectMany(category =>
            category.Apps.SelectMany(app => app.Navs.Select(nav => new
            {
                Category = category.Code,
                App = app.Code,
                Nav = nav
            })));

        foreach (var item in levelData)
        {
            result.Add(new CategoryAppNav(item.Category));
            result.Add(new CategoryAppNav(item.Category, item.App));
            result.Add(new CategoryAppNav(item.Category, item.App, item.Nav.Code));
            result.AddRange(CategoryAppNavs(item.Category, item.App, item.Nav));
        }

        return result;

        List<CategoryAppNav> CategoryAppNavs(string category, string app, Nav nav)
        {
            var childResult = new List<CategoryAppNav>();
            foreach (var item in nav.Children)
            {
                childResult.Add(new CategoryAppNav(category, app, item.Code));
                if (item.HasChildren)
                {
                    childResult.AddRange(CategoryAppNavs(category, app, item));
                }
            }
            return childResult;
        }
    }
}
