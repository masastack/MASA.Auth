// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using StackApp = Masa.Stack.Components.Models.App;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class PermissionsPreview
{
    List<Category> _categories = new();
    List<CategoryAppNav> _initValue = new();
    List<CategoryAppNav> _allData = new();
    List<string> _oldValue = new();
    string _idPrefix = RandomUtils.GenerateSpecifiedString(6);

    [Parameter]
    public List<string> Value { get; set; } = new();

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    ProjectService ProjectService => AuthCaller.ProjectService;

    protected override async Task OnParametersSetAsync()
    {
        if (Value.Count > 0 && !Value.SequenceEqual(_oldValue))
        {
            _oldValue = Value;
            await LoadData();
            _initValue = _allData.Where(i => Value.Contains(i.Nav ?? "")).ToList();
            StateHasChanged();
        }
        base.OnParametersSet();
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
        //todo repeat code ,wait ExpansionWrapper refactor
        _allData = DataConversion(_categories);
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
