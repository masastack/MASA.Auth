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
    public List<Guid> Roles { get; set; } = new();

    [Parameter]
    public List<Guid> Teams { get; set; } = new();

    [Parameter]
    public Guid User { get; set; }

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public bool Preview { get; set; }

    [Parameter]
    public EventCallback<bool> PreviewChanged { get; set; }

    List<Guid> _internalRoles = new();
    List<Guid> _internalTeams = new();
    Guid _internalUser;
    protected List<Category> _categories = new();

    List<Guid> RolePermissions { get; set; } = new();

    List<Guid> TeamPermission { get; set; } = new();

    protected List<Guid> RoleUnionTeamPermission { get; set; } = new();

    protected Dictionary<Guid, Guid> EmptyPermissionMap { get; set; } = new();

    protected virtual List<UniqueModel> ExpansionWrapperUniqueValue
    {
        get
        {
            var value = Value.Except(RoleUnionTeamPermission).Except(EmptyPermissionMap.Values);
            return value.Select(value => new UniqueModel(value.ToString(), false))
                        .Union(RoleUnionTeamPermission.Select(value => new UniqueModel(value.ToString(), true)))
                        .ToList();
        }
    }

    private ProjectService ProjectService => AuthCaller.ProjectService;

    private PermissionService PermissionService => AuthCaller.PermissionService;

    protected override async Task OnInitializedAsync()
    {
        await GetCategoriesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_internalRoles.SequenceEqual(Roles))
        {
            _internalRoles = Roles;
            await GetRolePermissions();
        }
        if (!_internalTeams.SequenceEqual(Teams))
        {
            _internalTeams = Teams;
            await GetTeamPermissions();
        }
        if (User != _internalUser)
        {
            _internalUser = User;
            await GetTeamPermissions();
        }
    }

    private async Task GetCategoriesAsync()
    {
        var apps = (await ProjectService.GetUIAndMenusAsync()).SelectMany(p => p.Apps).ToList();

        apps.RemoveAll(a => !a.Navs.Any());

        EmptyPermissionMap = apps.SelectMany(app => app.Navs)
                            .Where(nav => nav.PermissionType == default && nav.Children.Any(child => child.PermissionType == PermissionTypes.Menu))
                            .SelectMany(nav => nav.Children.Select(item => (Code: item.Code, ParentCode: nav.Code)))
                            .ToDictionary(item => Guid.Parse(item.Code), item => Guid.Parse(item.ParentCode));

        _categories = apps.GroupBy(a => a.Tag).Select(ag => new Category(ag.Key.Replace(" ", ""), ag.Key, ag.Select(a => a.Adapt<StackApp>()).ToList())).ToList();
    }

    private async Task GetRolePermissions()
    {
        RolePermissions = await PermissionService.GetPermissionsByRoleAsync(Roles);
        RoleUnionTeamPermission = TeamPermission.Union(RolePermissions).ToList();
        await RoleUnionTeamPermissionValueChangedAsync();
    }

    private async Task GetTeamPermissions()
    {
        TeamPermission = await PermissionService.GetPermissionsByTeamWithUserAsync(new(User, Teams));
        RoleUnionTeamPermission = TeamPermission.Union(RolePermissions).ToList();
        await RoleUnionTeamPermissionValueChangedAsync();
    }

    protected virtual Task RoleUnionTeamPermissionValueChangedAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual async Task ValueChangedAsync(List<UniqueModel> permissions)
    {
        var value = permissions.Select(permission => Guid.Parse(permission.Code)).Except(RoleUnionTeamPermission).ToList();
        foreach (var (code, parentCode) in EmptyPermissionMap)
        {
            if (value.Contains(code)) value.Add(parentCode);
        }
        await UpdateValueAsync(value.Distinct().ToList());
    }

    private async Task UpdateValueAsync(List<Guid> value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
