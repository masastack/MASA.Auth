// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddMenuPermission
{
    [EditorRequired]
    [Parameter]
    public List<AppDto> AppItems { get; set; } = new();

    [EditorRequired]
    [Parameter]
    public Guid ParentId { get; set; }

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<MenuPermissionDetailDto> OnSubmit { get; set; }

    [EditorRequired]
    [Parameter]
    public List<SelectItemDto<PermissionTypes>> SelectPermissionTypes { get; set; } = new();

    MenuPermissionDetailDto _menuPermissionDetailDto = new();
    MForm _form = default!;

    protected override void OnParametersSet()
    {
        _menuPermissionDetailDto.ParentId = ParentId;
        base.OnParametersSet();
    }

    private async Task OnSubmitHandler()
    {
        if (await _form.ValidateAsync())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_menuPermissionDetailDto);
            }
        }
    }
}
