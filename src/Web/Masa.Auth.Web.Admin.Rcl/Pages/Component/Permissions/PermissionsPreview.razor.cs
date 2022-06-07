// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using StackApp = Masa.Stack.Components.Models.App;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class PermissionsPreview
{
    List<Category> _categories = new();
    List<CategoryAppNav> _initValue = new();
    List<CategoryAppNav> _allData = new();
    string _idPrefix = RandomUtils.GenerateSpecifiedString(6);

    [Parameter]
    public List<string> Value { get; set; } = new();

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    ProjectService ProjectService => AuthCaller.ProjectService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            //_initValue.AddRange(_allData.Where(i => Value.Contains(i.Nav ?? "")));
            //StateHasChanged();
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
}
