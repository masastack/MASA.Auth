// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

using StackApp = Stack.Components.Models.App;

public partial class PermissionsConfigure
{
    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public List<Guid> RoleIds { get; set; } = new();

    private List<Guid> InternalRoleIds { get; set; } = new();

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    private List<Category> Categories { get; set; } = new();

    private ProjectService ProjectService => AuthCaller.ProjectService;

    private RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        await GetCategoriesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RoleIds.Count != InternalRoleIds.Count || RoleIds.Except(InternalRoleIds).Count() > 0)
        {
            InternalRoleIds = RoleIds;
            var permissions = await GetRolePermissions();
            var value = new List<Guid>();
            value.AddRange(Value);
            value.AddRange(permissions);
            await UpdateValueAsync(value.Distinct().ToList());
        }
    }

    private async Task GetCategoriesAsync()
    {
        var apps = (await ProjectService.GetListAsync(true)).SelectMany(p => p.Apps).ToList();
        Categories = apps.GroupBy(a => a.Tag).Select(ag => new Category
        {
            Code = ag.Key.Replace(" ", ""),
            Name = ag.Key,
            Apps = ag.Select(a => a.Adapt<StackApp>()).ToList()
        }).ToList();
    }

    private async Task<List<Guid>> GetRolePermissions() => await RoleService.GetPermissionsByRoleAsync(RoleIds);

    private async Task ValueChangedAsync(List<string> permissions)
    {
        await UpdateValueAsync(permissions.Select(permission => Guid.Parse(permission)).ToList());
    }

    private async Task UpdateValueAsync(List<Guid> value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
