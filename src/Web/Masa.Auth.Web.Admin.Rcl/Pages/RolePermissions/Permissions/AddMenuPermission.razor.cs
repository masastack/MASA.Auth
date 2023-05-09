// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddMenuPermission
{
    [Parameter]
    public EventCallback<MenuPermissionDetailDto> OnSubmit { get; set; }

    private MenuPermissionDetailDto _menuPermissionDetailDto = new();
    private MForm _form = default!;
    private bool _visible = false;
    private string _showUrlPrefix = "";
    private PermissionTypes _parentType;

    private List<SelectItemDto<PermissionTypes>> _menuPermissionTypes = new();

    private PermissionService PermissionService => AuthCaller.PermissionService;

    protected override void OnInitialized()
    {
        PageName = "PermissionBlock";
        base.OnInitialized();
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var permissionTypes = await PermissionService.GetTypesAsync();
            _menuPermissionTypes = permissionTypes.Where(a => a.Value != PermissionTypes.Api).ToList();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSubmitHandler()
    {
        if (_form.Validate())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_menuPermissionDetailDto);
            }
            _visible = false;
        }
    }

    public void Show(AppPermissionsViewModel appPermissionsViewModel)
    {
        _form?.Reset();
        _menuPermissionDetailDto = new()
        {
            AppId = appPermissionsViewModel.AppId,
            ParentId = appPermissionsViewModel.IsPermission ? appPermissionsViewModel.Id : Guid.Empty
        };
        _showUrlPrefix = appPermissionsViewModel.AppUrl;
        _visible = true;
    }

    private List<SelectItemDto<PermissionTypes>> TypeList()
    {
        if (_menuPermissionDetailDto.ParentId == Guid.Empty)
        {
            return _menuPermissionTypes.Where(p => p.Value == PermissionTypes.Menu).ToList();
        }
        if (_parentType == PermissionTypes.Element)
        {
            return _menuPermissionTypes.Where(p => p.Value != PermissionTypes.Menu).ToList();
        }
        return _menuPermissionTypes;
    }
}
