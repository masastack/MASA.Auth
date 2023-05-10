// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.
namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class PermissionsConfigure
{
    private bool _preview;
    
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
    public List<SubjectPermissionRelationDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<SubjectPermissionRelationDto>> ValueChanged { get; set; }

    [Parameter]
    public bool Preview
    {
        get => _preview;
        set
        {
            if (_menu != null)
            {
                _menu.Metadata.Situation =
                    value ? ExpansionMenuSituation.Preview : ExpansionMenuSituation.Authorization;
                _menu.SetHiddenByPreview(value);
            }

            _preview = value;
        }
    }

    [Parameter]
    public EventCallback<bool> PreviewChanged { get; set; }

    private List<Guid> _internalRoles = new();
    private List<Guid> _internalTeams = new();
    private Guid _internalUser;

    private List<Guid> RolePermissions { get; set; } = new();

    private List<Guid> TeamPermission { get; set; } = new();

    protected List<Guid> RoleUnionTeamPermission { get; set; } = new();

    private ExpansionMenu? _menu;

    private ProjectService ProjectService => AuthCaller.ProjectService;

    private PermissionService PermissionService => AuthCaller.PermissionService;

    protected override async Task OnInitializedAsync()
    {
        _menu = await GetMenuAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        var load = false;
        
        if (!_internalRoles.SequenceEqual(Roles))
        {
            load = true;
            _internalRoles = Roles;
            await GetRolePermissions();
        }

        if (!_internalTeams.SequenceEqual(Teams))
        {
            load = true;
            _internalTeams = Teams;
            await GetTeamPermissions();
        }

        if (User != _internalUser)
        {
            load = true;
            _internalUser = User;
            await GetTeamPermissions();
        }

        if (load)
        {
            _menu = await GetMenuAsync();
        }
    }

    private async Task<ExpansionMenu> GetMenuAsync()
    {
        var apps = (await ProjectService.GetUIAndMenusAsync()).SelectMany(p => p.Apps).ToList();
        apps.RemoveAll(a => !a.Navs.Any());
        var categories = apps.GroupBy(a => a.Tag);
        
        var permissionDict = Value.Select(p => new Tuple<string,bool>(p.PermissionId.ToString(), p.Effect))
            .Union(RoleUnionTeamPermission.Except(Value.Select(p=>p.PermissionId)).Select(p => new Tuple<string,bool>(p.ToString(), true)))
            .ToDictionary(p => p.Item1);

        var rootMenu = ExpansionMenu.CreateRootMenu(ExpansionMenuSituation.Authorization);
        try
        {
            foreach (var category in categories)
            {
                var categoryMenu = new ExpansionMenu(category.Key, category.Key, ExpansionMenuType.Category, ExpansionMenuState.Normal, rootMenu.Metadata, parent: rootMenu);
                foreach (var app in category)
                {
                    var state = GetMenuState(permissionDict, app.Id.ToString());
                    var appMenu = new ExpansionMenu(app.Id.ToString(), app.Name, ExpansionMenuType.App, state, rootMenu.Metadata, parent: categoryMenu);
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
    
    private ExpansionMenu ConvertForNav(PermissionNavDto navModel, int deep, ExpansionMenu parent,IDictionary<string,Tuple<string,bool>> permissionDict)
    {
        var state = GetMenuState(permissionDict, navModel.Code);
        var impersonal = Impersonal(permissionDict, navModel.Code);
        var type = ExpansionMenuType.Nav;
        if (navModel.PermissionType == PermissionTypes.Element)
        {
            type = ExpansionMenuType.Element;
        }
        var menu = new ExpansionMenu(navModel.Code, navModel.Name, type, state, parent.Metadata, impersonal, parent: parent, stateChangedAsync: MenuStateChangedAsync);
        foreach (var childrenNav in navModel.Children)
        {
            menu.AddChild(ConvertForNav(childrenNav, deep++, menu, permissionDict));
        }
        return menu;
    }

    private ExpansionMenuState GetMenuState(IDictionary<string, Tuple<string,bool>> permissionDict, string menuId)
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

        permission.Effect = menu.State == ExpansionMenuState.Selected;
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
}
