// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddApiPermission
{
    [Parameter]
    public EventCallback<ApiPermissionDetailDto> OnSubmit { get; set; }

    MForm _form = default!;
    ApiPermissionDetailDto _apiPermissionDetailDto = new();
    bool _visible { get; set; }
    List<SelectItemDto<PermissionTypes>> _apiPermissionTypes = new();
    string _showUrlPrefix = "";

    PermissionService PermissionService => AuthCaller.PermissionService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var permissionTypes = await PermissionService.GetTypesAsync();
            _apiPermissionTypes = permissionTypes.Where(a => a.Value == PermissionTypes.Api).ToList();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSubmitHandler()
    {
        if (_form.Validate())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_apiPermissionDetailDto);
            }
            _visible = false;
        }
    }

    public void Show(AppPermissionsViewModel appPermissionsViewModel)
    {
        _form?.Reset();
        _apiPermissionDetailDto.AppId = appPermissionsViewModel.AppId;
        _showUrlPrefix = appPermissionsViewModel.AppUrl;
        _visible = true;
    }
}
