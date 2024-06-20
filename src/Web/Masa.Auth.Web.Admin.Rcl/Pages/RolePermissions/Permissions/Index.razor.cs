// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    private string _tab = "", _search = "";

    private bool _showMenuInfo, _showApiInfo;

    private List<AppPermissionsViewModel> _menuPermissions = new(), _apiPermissions = new();

    private List<Guid> _menuPermissionActive = new(), _apiPermissionActive = new()
        , _menuOpenNode = new(), _apiOpenNode = new();

    private string _curProjectId = "";

    private MenuPermissionDetailDto _menuPermissionDetailDto = new();

    private ApiPermissionDetailDto _apiPermissionDetailDto = new();

    private AppGlobalNavVisibleDto _appGlobalNavVisibleDto = new();

    private List<ProjectDto> _projectItems = new();

    private List<AppDto> _curAppItems = new();

    private List<SelectItemDto<Guid>> _childApiItems = new();

    private MForm _formMenu = default!, _formApi = default!, _formMenuApp = default!;

    private AddMenuPermission _addMenuPermission = null!;

    private AddApiPermission _addApiPermission = null!;

    private string? _lastProjectId = null;

    private readonly Dictionary<string, Guid> _appIds = new();

    private PermissionService PermissionService => AuthCaller.PermissionService;

    private ProjectService ProjectService => AuthCaller.ProjectService;

    private List<SelectItemDto<PermissionTypes>> _permissionTypes = new();

    private string _showUrlPrefix = "";

    private bool _disableMenuUrl = false;

    protected override void OnInitialized()
    {
        PageName = "PermissionBlock";
        base.OnInitialized();
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _permissionTypes = await PermissionService.GetTypesAsync();
            _tab = "0";
            _projectItems = await ProjectService.GetListAsync();
            if (!_projectItems.Any())
            {
                return;
            }
            await SelectProjectItem(_projectItems.First());
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private List<AppPermissionsViewModel> MenuTreeParent(MenuPermissionDetailDto menuPermissionDetailDto)
    {
        var menus = _menuPermissions.Where(m => m.AppId == menuPermissionDetailDto.AppId)
            .Select(m => (AppPermissionsViewModel)m.Clone()).ToList();
        menus.ForEach(item =>
        {
            item.Id = Guid.Empty;
        });
        RemoveAll(menus, x => x.Id == menuPermissionDetailDto.Id);
        if (menuPermissionDetailDto.Type == PermissionTypes.Menu)
        {
            RemoveChildElementAll(menus);
            RemoveAll(menus, p => p.Type != null && p.Type != PermissionTypes.Menu);
        }
        return menus;

        void RemoveAll(List<AppPermissionsViewModel> menus, Predicate<AppPermissionsViewModel> match)
        {
            menus.RemoveAll(match);
            foreach (var menu in menus)
            {
                RemoveAll(menu.Children, match);
            }
        }

        void RemoveChildElementAll(List<AppPermissionsViewModel> menus)
        {
            foreach (var menu in menus.ToArray())
            {
                if (menu.Children?.Any() == true)
                {
                    if (menu.Children.Any(x => x.Type == PermissionTypes.Element))
                    {
                        RemoveAll(menus, x => x.Id == menu.Id);
                    }
                    else
                    {
                        RemoveChildElementAll(menu.Children);
                    }
                }
            }
        }
    }

    private async Task SelectProjectItem(ProjectDto project)
    {
        _lastProjectId = _curProjectId;
        _curAppItems = project.Apps;
        _curProjectId = project.Identity;
        if (_lastProjectId != _curProjectId)
        {
            SetAppIds(_curAppItems);
        }
        _childApiItems = await PermissionService.GetApiPermissionSelectAsync(_curProjectId);
        await InitAppPermissionsAsync();
    }

    private void SetAppIds(List<AppDto> curAppItems)
    {
        _appIds.Clear();
        foreach (var app in curAppItems)
        {
            _appIds.TryAdd(app.Identity, Guid.NewGuid());
        }
    }

    private async Task InitAppPermissionsAsync(PermissionDetailDto? activeDto = null)
    {
        var menuPermissions = _curAppItems.Where(a => a.Type == AppTypes.UI).Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            AppTag = a.Tag,
            Id = _appIds[a.Identity],
            AppUrl = a.Url,
            Name = a.Name
        }).ToList();

        var apiPermissions = _curAppItems.Where(a => a.Type == AppTypes.Service).Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            AppTag = a.Tag,
            Id = _appIds[a.Identity],
            AppUrl = a.Url,
            Name = a.Name
        }).ToList();

        var applicationPermissions = await PermissionService.GetApplicationPermissionsAsync(_curProjectId);

        var config = new TypeAdapterConfig();
        config.NewConfig<AppPermissionDto, AppPermissionsViewModel>()
            .Map(dest => dest.Id, src => src.PermissionId)
            .Map(dest => dest.Name, src => src.PermissionName)
            .Map(dest => dest.IsPermission, src => true)
            .Map(dest => dest.AppUrl, src => MapContext.Current == null ? "" : MapContext.Current.Parameters["appUrl"]);

        menuPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => (p.Type == PermissionTypes.Menu || p.Type == PermissionTypes.Element) && p.AppId == mp.AppId);
            mp.Children.AddRange(permissions
                .BuildAdapter(config)
                .AddParameters("appUrl", mp.AppUrl)
                .AdaptToType<List<AppPermissionsViewModel>>());
        });
        apiPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => p.Type == PermissionTypes.Api && p.AppId == mp.AppId);
            mp.Children.AddRange(permissions
                .BuildAdapter(config)
                .AddParameters("appUrl", mp.AppUrl)
                .AdaptToType<List<AppPermissionsViewModel>>());
        });

        _menuPermissions = menuPermissions;
        _apiPermissions = apiPermissions;
        //set active and open
        ActiveMenuPermissionThenOpen();

        void ActiveMenuPermissionThenOpen()
        {
            _apiOpenNode = _apiPermissions.Select(m => m.Id).ToList();
            if (activeDto != null)
            {
                if (activeDto is MenuPermissionDetailDto menu)
                {
                    List<dynamic> flattenList = new();
                    RecursiveFlatten(_menuPermissions.First(x => x.AppId == menu.AppId), flattenList);
                    List<Guid> parents = new();
                    GetParentFromId(menu.Id, parents);
                    NextTick(() =>
                    {
                        _menuOpenNode = parents;
                        _menuPermissionActive = new List<Guid> { menu.Id };
                        _menuPermissionDetailDto = menu;
                        StateHasChanged();
                    });
                    void GetParentFromId(Guid id, in List<Guid> parentIds)
                    {
                        foreach (var item in flattenList)
                        {
                            if (item.Id == id)
                            {
                                parentIds.Add(item.ParentId);
                                GetParentFromId(item.ParentId, parentIds);
                            }
                        }
                    }
                }
                else if (activeDto is ApiPermissionDetailDto api)
                {
                    NextTick(() =>
                    {
                        _apiPermissionActive = new List<Guid> { api.Id };
                        _apiPermissionDetailDto = api;
                        StateHasChanged();
                    });
                }
            }
            else
            {
                _menuOpenNode = _menuPermissions.Select(m => m.Id).ToList();//open root
                if (!_menuPermissionActive.Any())
                {
                    _menuPermissionActive = _menuOpenNode.Take(1).ToList();
                }
                //set active and open
                if (!_apiPermissionActive.Any())
                {
                    _apiPermissionActive = _apiOpenNode.Take(1).ToList();
                }
            }
        }

        void RecursiveFlatten(AppPermissionsViewModel vm, in List<dynamic> flattenList)
        {
            if (vm.Children.Any())
            {
                foreach (var child in vm.Children)
                {
                    flattenList.Add(new
                    {
                        Id = child.Id,
                        ParentId = vm.Id
                    });
                    RecursiveFlatten(child, flattenList);
                }
            }
        }
    }

    private async Task ActiveMenuPermission(List<AppPermissionsViewModel> activeItems)
    {
        if (activeItems.Any())
        {
            _showMenuInfo = true;
            var curItem = activeItems.First();
            _disableMenuUrl = curItem.Children.Any(e => e.Type == PermissionTypes.Menu);
            if (curItem.IsPermission)
            {
                _menuPermissionDetailDto = await PermissionService.GetMenuPermissionDetailAsync(curItem.Id);
            }
            else
            {
                _menuPermissionDetailDto = new();

                if (!curItem.AppId.IsNullOrEmpty())
                {
                    _appGlobalNavVisibleDto = await PermissionService.GetAppGlobalNavVisibleAsync(curItem.AppId);
                }
            }
            _showUrlPrefix = curItem.AppUrl.EnsureTrailingSlash();
        }
        else
        {
            _showMenuInfo = false;
        }
    }

    private async Task ActiveApiPermission(List<AppPermissionsViewModel> activeItems)
    {
        if (activeItems.Any())
        {
            var curItem = activeItems.First();
            _showApiInfo = true;
            if (curItem.IsPermission)
            {
                _apiPermissionDetailDto = await PermissionService.GetApiPermissionDetailAsync(curItem.Id);
            }
            else
            {
                _apiPermissionDetailDto = new();
            }
            _showUrlPrefix = curItem.AppUrl.EnsureTrailingSlash();
        }
        else
        {
            _showApiInfo = false;
        }
    }

    private async Task AddMenuPermissionAsync(MenuPermissionDetailDto dto)
    {
        if (string.IsNullOrWhiteSpace(_curProjectId))
        {
            OpenErrorMessage(T("Project identifier is empty"));
            return;
        }
        dto.SystemId = _curProjectId;
        var resultDto = await PermissionService.UpsertMenuPermissionAsync(dto);
        await InitAppPermissionsAsync(resultDto);
        OpenSuccessMessage(T("Add menu permission data success"));
    }

    private async Task AddApiPermissionAsync(ApiPermissionDetailDto dto)
    {
        if (string.IsNullOrWhiteSpace(_curProjectId))
        {
            OpenErrorMessage(T("Project identifier is empty"));
            return;
        }
        dto.SystemId = _curProjectId;
        var resultDto = await PermissionService.UpsertApiPermissionAsync(dto);
        await InitAppPermissionsAsync(resultDto);
        OpenSuccessMessage(T("Add api permission data success"));
    }

    private async Task UpdateMenuPermissionAsync()
    {
        if (_formMenu.Validate())
        {
            _menuPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertMenuPermissionAsync(_menuPermissionDetailDto);
            OpenSuccessMessage(T("Edit menu permission data success"));
            await InitAppPermissionsAsync();
        }
    }

    private async Task UpdateApiPermissionAsync()
    {
        if (_formApi.Validate())
        {
            _apiPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertApiPermissionAsync(_apiPermissionDetailDto);
            OpenSuccessMessage(T("Edit api permission data success"));
            await InitAppPermissionsAsync();
        }
    }

    private async Task SaveAppGlobalNavVisibleAsync()
    {
        if (_formMenuApp.Validate())
        {
            await PermissionService.SaveAppGlobalNavVisibleAsync(_appGlobalNavVisibleDto);
            OpenSuccessMessage(T("SaveAppGlobalNavVisibleSuccess"));
        }
    }

    private async Task DeletePermissionAsync(PermissionDetailDto permission)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Permission"), T("Are you sure to delete permission {0}", DT(permission.Name)));
        if (isConfirmed)
        {
            await PermissionService.RemoveAsync(permission.Id);
            var parentId = permission.ParentId;
            if (parentId == Guid.Empty)
            {
                if (permission is MenuPermissionDetailDto)
                {
                    var m = _menuPermissions.FirstOrDefault(x => x.Children.Any(x => x.Id == permission.Id));
                    if (m != null)
                    {
                        parentId = m.Id;
                    }
                }
            }
            await InitAppPermissionsAsync();
            if (permission is MenuPermissionDetailDto)
            {
                _menuPermissionActive = new() { parentId };
            }
            else if (permission is ApiPermissionDetailDto)
            {
                if (parentId == Guid.Empty)
                {
                    parentId = _apiPermissions.First().Id;
                }
                _apiPermissionActive = new() { parentId };
            }
        }
    }
}
