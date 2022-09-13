// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class Index
{
    string _tab = "", _search = "";
    bool _showMenuInfo, _showApiInfo;
    List<AppPermissionsViewModel> _menuPermissions = new();
    List<AppPermissionsViewModel> _apiPermissions = new();
    List<Guid> _menuPermissionActive = new List<Guid>();
    List<Guid> _apiPermissionActive = new List<Guid>();
    string _curProjectId = "";
    MenuPermissionDetailDto _menuPermissionDetailDto = new();
    ApiPermissionDetailDto _apiPermissionDetailDto = new();
    List<ProjectDto> _projectItems = new();
    List<AppDto> _curAppItems = new();
    List<SelectItemDto<Guid>> _childApiItems = new();
    MForm _formMenu = default!, _formApi = default!;
    List<string> _appTags = new();
    AppTagDetailDto _appTagDto = new();
    AddMenuPermission _addMenuPermission = null!;
    AddApiPermission _addApiPermission = null!;

    PermissionService PermissionService => AuthCaller.PermissionService;

    ProjectService ProjectService => AuthCaller.ProjectService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _tab = T("Menu Permission");

            _projectItems = await ProjectService.GetListAsync();
            if (!_projectItems.Any())
            {
                return;
            }
            await SelectProjectItem(_projectItems.First());
            await GetAppTags();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetAppTags()
    {
        if (!_appTags.Any())
        {
            _appTags = await ProjectService.GetAppTagsAsync();
        }
    }

    private async Task SelectProjectItem(ProjectDto project)
    {
        _curAppItems = project.Apps;
        _curProjectId = project.Identity;
        _childApiItems = await PermissionService.GetApiPermissionSelectAsync(_curProjectId);
        await InitAppPermissions();
    }

    private async Task InitAppPermissions()
    {
        _menuPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            AppTag = a.Tag,
            Id = Guid.NewGuid(),
            AppUrl = a.Url,
            Name = a.Name
        }).ToList();
        _menuPermissionActive = _menuPermissions.Select(m => m.Id).Take(1).ToList();
        _apiPermissions = _curAppItems.Select(a => new AppPermissionsViewModel
        {
            IsPermission = false,
            AppId = a.Identity,
            AppTag = a.Tag,
            Id = Guid.NewGuid(),
            AppUrl = a.Url,
            Name = a.Name
        }).ToList();
        _apiPermissionActive = _apiPermissions.Select(m => m.Id).Take(1).ToList();
        var applicationPermissions = await PermissionService.GetApplicationPermissionsAsync(_curProjectId);

        var config = new TypeAdapterConfig();
        config.NewConfig<AppPermissionDto, AppPermissionsViewModel>().Map(dest => dest.Id, src => src.PermissonId)
            .Map(dest => dest.Name, src => src.PermissionName)
            .Map(dest => dest.IsPermission, src => true)
            .Map(dest => dest.AppUrl, src => MapContext.Current == null ? "" : MapContext.Current.Parameters["appUrl"]);

        _menuPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => p.Type == PermissionTypes.Menu && p.AppId == mp.AppId);
            mp.Children.AddRange(permissions
                .BuildAdapter(config)
                .AddParameters("appUrl", mp.AppUrl)
                .AdaptToType<List<AppPermissionsViewModel>>());
        });
        _apiPermissions.ForEach(mp =>
        {
            var permissions = applicationPermissions.Where(p => p.Type == PermissionTypes.Api && p.AppId == mp.AppId);
            mp.Children.AddRange(permissions
                .BuildAdapter(config)
                .AddParameters("appUrl", mp.AppUrl)
                .AdaptToType<List<AppPermissionsViewModel>>());
        });
    }

    private async Task ActiveMenuPermission(List<AppPermissionsViewModel> activeItems)
    {
        if (activeItems.Any())
        {
            _showMenuInfo = true;
            var curItem = activeItems.First();
            if (curItem.IsPermission)
            {
                _appTagDto = new AppTagDetailDto();
                _menuPermissionDetailDto = await PermissionService.GetMenuPermissionDetailAsync(curItem.Id);
            }
            else
            {
                _appTagDto = new AppTagDetailDto
                {
                    AppIdentity = curItem.AppId,
                    Tag = curItem.AppTag
                };
                _menuPermissionDetailDto = new();
            }
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
                _appTagDto = new();
                _apiPermissionDetailDto = await PermissionService.GetApiPermissionDetailAsync(curItem.Id);
            }
            else
            {
                _appTagDto = new AppTagDetailDto
                {
                    AppIdentity = curItem.AppId,
                    Tag = curItem.AppTag
                };
                _apiPermissionDetailDto = new();
            }
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
        await PermissionService.UpsertMenuPermissionAsync(dto);
        if (!dto.IsUpdate)
        {
            await InitAppPermissions();
        }
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
        await PermissionService.UpsertApiPermissionAsync(dto);
        if (!dto.IsUpdate)
        {
            await InitAppPermissions();
        }
        OpenSuccessMessage(T("Add api permission data success"));
    }

    private async Task UpdateMenuPermissionAsync()
    {
        if (_formMenu.Validate())
        {
            _menuPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertMenuPermissionAsync(_menuPermissionDetailDto);
            OpenSuccessMessage(T("Edit menu permission data success"));
        }
    }

    private async Task UpdateApiPermissionAsync()
    {
        if (_formApi.Validate())
        {
            _apiPermissionDetailDto.SystemId = _curProjectId;
            await PermissionService.UpsertApiPermissionAsync(_apiPermissionDetailDto);
            OpenSuccessMessage(T("Edit api permission data success"));
        }
    }

    private async Task DeletePermissionAsync(Guid id)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Permission"), T("Are you sure you want to delete this permission"), AlertTypes.Warning);
        if (isConfirmed)
        {
            await PermissionService.RemoveAsync(id);
            await InitAppPermissions();
        }
    }

    private async Task SaveAppTag(AppTagDetailDto appTagDto)
    {
        if (string.IsNullOrEmpty(appTagDto.Tag) || string.IsNullOrEmpty(appTagDto.AppIdentity))
        {
            OpenWarningMessage(T("Tag or appid is empty"));
            return;
        }
        await ProjectService.SaveAppTagAsync(appTagDto);
    }
}
