using Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions.ViewModels;

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    StringNumber _tab = 0;
    bool _enabled = false;
    List<AppPermissionsViewModel> _menuPermissions = new();
    List<AppPermissionsViewModel> _apiPermissions = new();
    List<Guid> _menuPermissionActive = new List<Guid>();
    List<Guid> _apiPermissionActive = new List<Guid>();
    int _curProjectId;
    bool _addApiPermission, _addMenuPermission;
    MenuPermissionDetailDto _menuPermissionDetailDto = new();
    ApiPermissionDetailDto _apiPermissionDetailDto = new();
    List<SelectItemDto<PermissionTypes>> _menuPermissionTypes = new();
    List<SelectItemDto<PermissionTypes>> _apiPermissionTypes = new();
    List<ProjectDto> _projectItems = new();
    List<AppDto> _curAppItems = new();
    List<SelectItemDto<Guid>> _childApiItems = new();

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
            _curProjectId = _projectItems.First().Id;
            _curAppItems = _projectItems.First().Apps;

            InitAppPermissions();

            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void SelectProjectItem(ProjectDto project)
    {
        _curAppItems = project.Apps;

        InitAppPermissions();
    }

    //await PermissionService.GetApplicationPermissionsAsync(_curProjectId);

    private void InitAppPermissions()
    {
        _menuPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = true,
            Id = Guid.NewGuid(),
            Name = a.Name
        }).ToList();
        _apiPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = true,
            Id = Guid.NewGuid(),
            Name = a.Name
        }).ToList();
    }
}
