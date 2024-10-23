// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class PermissionsConfigure
{
    [Parameter]
    public List<Guid> Roles { get; set; } = new();

    [Parameter]
    public List<Guid> Teams { get; set; } = new();

    [Parameter]
    public Guid User { get; set; }

    [Parameter]
    public List<SubjectPermissionRelationDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<SubjectPermissionRelationDto>> ValueChanged { get; set; }

    [Parameter]
    public bool HasPreview { get; set; }

    [Parameter]
    public bool Preview { get; set; }

    [Parameter]
    public EventCallback<bool> PreviewChanged { get; set; }

    private List<Guid> _internalRoles = new();
    private List<Guid> _internalTeams = new();
    private Guid _internalUser;

    private bool _shouldRender = true;
    private List<Guid> _prevEffectIds = new();
    private List<Guid> _prevNoEffectIds = new();
    private List<Guid> RolePermissions { get; set; } = new();

    private List<Guid> TeamPermission { get; set; } = new();

    protected List<Guid> RoleUnionTeamPermission { get; set; } = new();

    private ExpansionMenu? _menu;

    private ExpansionMenu? Menu
    {
        get
        {
            if (_menu != null)
            {
                _menu.SetSituation(ExpansionMenuSituation.Authorization);
                _menu.SetHiddenByPreview(false);
            }

            return _menu;
        }
    }

    private ExpansionMenu? PreviewMenu
    {
        get
        {
            var previewMenu = (ExpansionMenu?)_menu?.Clone();
            if (previewMenu != null)
            {
                previewMenu.SetSituation(ExpansionMenuSituation.Preview);
                previewMenu.SetHiddenByPreview(true);
            }

            return previewMenu;
        }
    }

    private ProjectService ProjectService => AuthCaller.ProjectService;

    private PermissionService PermissionService => AuthCaller.PermissionService;

    protected async override Task OnParametersSetAsync()
    {
        var load = false;

        var effectIds = Value.Where(u => u.Effect).Select(u => u.PermissionId).ToList();
        if (!_prevEffectIds.SequenceEqual(effectIds))
        {
            _prevEffectIds = effectIds;
            _shouldRender = true;
        }

        var noEffectIds = Value.Where(u => u.Effect == false).Select(u => u.PermissionId).ToList();
        if (!_prevNoEffectIds.SequenceEqual(noEffectIds))
        {
            _prevNoEffectIds = noEffectIds;
            _shouldRender = true;
        }

        if (!_internalRoles.SequenceEqual(Roles))
        {
            _shouldRender = true;
            load = true;
            _internalRoles = Roles;
            await GetRolePermissions();
        }

        if (!_internalTeams.SequenceEqual(Teams))
        {
            _shouldRender = true;
            load = true;
            _internalTeams = Teams;
            await GetTeamPermissions();
        }

        if (User != _internalUser)
        {
            _shouldRender = true;
            load = true;
            _internalUser = User;
            await GetTeamPermissions();
        }

        if (load || _menu is null)
        {
            _menu = await GetMenuAsync();
            _shouldRender = true;
        }
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Preview), out bool preview) && preview != Preview)
        {
            _shouldRender = true;
        }
        return base.SetParametersAsync(parameters);
    }

    protected override bool ShouldRender()
    {
        return _shouldRender;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (_shouldRender)
        {
            _shouldRender = false;
        }
    }

    private async Task<ExpansionMenu> GetMenuAsync()
    {
        //TODO repeat code with Masa.Stack.Components.Shared.GlobalNavigations.GlobalNavigation
        var apps = (await ProjectService.GetUIAndMenusAsync()).SelectMany(p => p.Apps).ToList();
        apps.RemoveAll(a => !a.Navs.Any());
        var categories = apps.GroupBy(a => a.Tag);

        var permissionDict = Value.Select(p => new Tuple<string, bool>(p.PermissionId.ToString(), p.Effect))
            .Union(RoleUnionTeamPermission.Except(Value.Select(p => p.PermissionId)).Select(p => new Tuple<string, bool>(p.ToString(), true)))
            .ToDictionary(p => p.Item1);

        var rootMenu = ExpansionMenu.CreateRootMenu(ExpansionMenuSituation.Authorization);
        try
        {
            foreach (var category in categories)
            {
                var categoryMenu = new ExpansionMenu(category.Key, category.Key, ExpansionMenuType.Category, ExpansionMenuState.Normal, rootMenu.MetaData, parent: rootMenu);
                foreach (var app in category)
                {
                    var state = GetMenuState(permissionDict, app.Id.ToString());
                    var appMenu = new ExpansionMenu(app.Id.ToString(), app.Name, ExpansionMenuType.App, state, rootMenu.MetaData, parent: categoryMenu);
                    foreach (var nav in app.Navs)
                    {
                        appMenu.AddChild(ConvertForNav(nav, appMenu.Deep + 1, appMenu, permissionDict));
                    }
                    categoryMenu.AddChild(appMenu);
                }
                rootMenu.AddChild(categoryMenu);
            }
        }
        catch
        {
        }

        return rootMenu;
    }

    private ExpansionMenu ConvertForNav(PermissionNavDto navModel, int deep, ExpansionMenu parent, IDictionary<string, Tuple<string, bool>> permissionDict)
    {
        var state = GetMenuState(permissionDict, navModel.Code);
        var impersonal = Impersonal(permissionDict, navModel.Code);
        var type = ExpansionMenuType.Nav;
        if (navModel.PermissionType == PermissionTypes.Element)
        {
            type = ExpansionMenuType.Element;
        }
        var menu = new ExpansionMenu(navModel.Code, navModel.Name, type, state, parent.MetaData, impersonal, parent: parent, stateChangedAsync: MenuStateChangedAsync);
        foreach (var childrenNav in navModel.Children)
        {
            menu.AddChild(ConvertForNav(childrenNav, deep++, menu, permissionDict));
        }
        return menu;
    }

    private ExpansionMenuState GetMenuState(IDictionary<string, Tuple<string, bool>> permissionDict, string menuId)
    {
        if (!permissionDict.TryGetValue(menuId, out var permission))
        {
            return ExpansionMenuState.Normal;
        }

        if (!RoleUnionTeamPermission.Contains(Guid.Parse(menuId)) && !permission.Item2)
        {
            return ExpansionMenuState.Normal;
        }

        return permission.Item2 ? ExpansionMenuState.Selected : ExpansionMenuState.Impersonal;
    }

    private bool Impersonal(IDictionary<string, Tuple<string, bool>> permissionDict, string menuId)
    {
        return permissionDict.ContainsKey(menuId) && RoleUnionTeamPermission.Contains(Guid.Parse(menuId));
    }

    private Task MenuStateChangedAsync(ExpansionMenu menu)
    {
        var menuId = Guid.Parse(menu.Id);
        var permission = Value.FirstOrDefault(p => p.PermissionId == menuId);
        if (permission == null)
        {
            permission = new SubjectPermissionRelationDto(menuId, false);
            Value.Add(permission);
        }

        permission.Effect = menu.State == ExpansionMenuState.Selected || menu.State == ExpansionMenuState.Indeterminate;
        return Task.CompletedTask;
    }

    private async Task GetRolePermissions()
    {
        RolePermissions = await PermissionService.GetPermissionsByRoleAsync(Roles);
        RoleUnionTeamPermission = TeamPermission.Union(RolePermissions).ToList();
    }

    private async Task GetTeamPermissions()
    {
        TeamPermission = await PermissionService.GetPermissionsByTeamWithUserAsync(new(User, Teams));
        RoleUnionTeamPermission = TeamPermission.Union(RolePermissions).ToList();
    }

    public void Reset()
    {
        _menu = null;
    }
}
