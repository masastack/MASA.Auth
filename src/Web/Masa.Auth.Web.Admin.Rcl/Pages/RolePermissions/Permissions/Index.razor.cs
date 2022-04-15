namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    StringNumber _tab = 0;
    bool _searchApiLoading, _showMenuInfo, _showApiInfo;
    List<AppPermissionsViewModel> _menuPermissions = new();
    List<AppPermissionsViewModel> _apiPermissions = new();
    List<Guid> _menuPermissionActive = new List<Guid>();
    List<Guid> _apiPermissionActive = new List<Guid>();
    string _curProjectId = "";
    bool _addApiPermission, _addMenuPermission;
    MenuPermissionDetailDto _menuPermissionDetailDto = new();
    ApiPermissionDetailDto _apiPermissionDetailDto = new();
    List<SelectItemDto<PermissionTypes>> _menuPermissionTypes = new();
    List<SelectItemDto<PermissionTypes>> _apiPermissionTypes = new();
    List<ProjectDto> _projectItems = new();
    List<AppDto> _curAppItems = new();
    List<SelectItemDto<Guid>> _childApiItems = new();
    Guid _parentMenuId;
    MForm _formMenu = default!, _formApi = default!;

    PermissionService PermissionService => AuthCaller.PermissionService;

    ProjectService ProjectService => AuthCaller.ProjectService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var permissionTypes = await PermissionService.GetTypesAsync();
            _menuPermissionTypes = permissionTypes.Where(a => a.Value != PermissionTypes.Api).ToList();
            _apiPermissionTypes = permissionTypes.Where(a => a.Value == PermissionTypes.Api).ToList();

            _projectItems = await ProjectService.GetListAsync();
            if (!_projectItems.Any())
            {
                return;
            }
            _curProjectId = _projectItems.First().Identity;
            _curAppItems = _projectItems.First().Apps;

            await InitAppPermissions();

            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task SelectProjectItem(ProjectDto project)
    {
        _curAppItems = project.Apps;

        await InitAppPermissions();
    }

    private async Task InitAppPermissions()
    {
        _menuPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            Id = Guid.NewGuid(),
            Name = a.Name
        }).ToList();
        _apiPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            Id = Guid.NewGuid(),
            Name = a.Name
        }).ToList();

        var applicationPermissions = await PermissionService.GetApplicationPermissionsAsync(_curProjectId);

        _menuPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => p.Type == PermissionTypes.Menu && p.AppId == mp.AppId)
            .Select(p => new AppPermissionsViewModel
            {
                IsPermission = true,
                Name = p.PermissonName,
                Id = p.PermissonId
            });
            mp.Children.AddRange(permissions);
        });
        _apiPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => p.Type == PermissionTypes.Api && p.AppId == mp.AppId)
            .Select(p => new AppPermissionsViewModel
            {
                IsPermission = true,
                Name = p.PermissonName,
                Id = p.PermissonId
            });
            mp.Children.AddRange(permissions);
        });
    }

    private async Task ActiveMenuPermission(List<AppPermissionsViewModel> activeItems)
    {
        if (activeItems.Any())
        {
            var curItem = activeItems.First();
            _showMenuInfo = curItem.IsPermission;
            if (curItem.IsPermission)
            {
                _menuPermissionDetailDto = await PermissionService.GetMenuPermissionDetailAsync(curItem.Id);
                if (!curItem.Children.Any())
                {
                    var childPermissions = await PermissionService.GetChildMenuPermissionsAsync(curItem.Id);
                    curItem.Children.AddRange(childPermissions.Select(a => new AppPermissionsViewModel
                    {
                        IsPermission = true,
                        Name = a.Name,
                        Id = a.Id
                    }));
                }
            }
            else
            {
                _menuPermissionDetailDto = new();
            }
        }
    }

    private async Task ActiveApiPermission(List<AppPermissionsViewModel> activeItems)
    {
        if (activeItems.Any())
        {
            var curItem = activeItems.First();
            _showApiInfo = curItem.IsPermission;
            if (curItem.IsPermission)
            {
                _apiPermissionDetailDto = await PermissionService.GetApiPermissionDetailAsync(curItem.Id);
            }
            else
            {
                _apiPermissionDetailDto = new();
            }
        }
    }

    private void AddMenuPermission(AppPermissionsViewModel appPermissionsViewModel)
    {
        _addMenuPermission = true;
        _parentMenuId = appPermissionsViewModel.IsPermission ? appPermissionsViewModel.Id : Guid.Empty;
    }

    private void AddApiPermission()
    {
        _addApiPermission = true;
    }

    private async Task UpdateSearchApiAsync(string val)
    {
        _searchApiLoading = true;
        _childApiItems = await PermissionService.GetApiPermissionSelectAsync(val);
        _searchApiLoading = false;
    }

    private async Task AddMenuPermissionAsync(MenuPermissionDetailDto dto)
    {
        if (string.IsNullOrWhiteSpace(_curProjectId))
        {
            OpenErrorMessage(I18n.T("Project identifier is empty"));
            return;
        }
        dto.SystemId = _curProjectId;
        await PermissionService.UpsertMenuPermissionAsync(dto);
        if (!dto.IsUpdate)
        {
            await InitAppPermissions();
        }
        _addMenuPermission = false;
        OpenSuccessMessage(I18n.T("Add menu permission data success"));
    }

    private async Task AddApiPermissionAsync(ApiPermissionDetailDto dto)
    {
        if (string.IsNullOrWhiteSpace(_curProjectId))
        {
            OpenErrorMessage(I18n.T("Project identifier is empty"));
            return;
        }
        dto.SystemId = _curProjectId;
        await PermissionService.UpsertApiPermissionAsync(dto);
        if (!dto.IsUpdate)
        {
            await InitAppPermissions();
        }
        _addApiPermission = false;
        OpenSuccessMessage(I18n.T("Add api permission data success"));
    }

    private async Task UpdateMenuPermissionAsync()
    {
        if (await _formMenu.ValidateAsync())
        {
            _menuPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertMenuPermissionAsync(_menuPermissionDetailDto);
            _addMenuPermission = false;
            OpenSuccessMessage(I18n.T("Edit menu permission data success"));
        }
    }

    private async Task UpdateApiPermissionAsync()
    {
        if (await _formApi.ValidateAsync())
        {
            _apiPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertApiPermissionAsync(_apiPermissionDetailDto);
            _addMenuPermission = false;
            OpenSuccessMessage(I18n.T("Edit api permission data success"));
        }
    }

    private async Task DeletePermissionAsync(Guid id)
    {
        var isConfirmed = await OpenConfirmDialog(I18n.T("Delete Permission"), I18n.T("Are you sure you want to delete this permission"), AlertTypes.Warning);
        if (isConfirmed)
        {
            await PermissionService.RemoveAsync(id);
            await InitAppPermissions();
        }
    }
}
