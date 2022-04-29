// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions;

public partial class AddApiPermission
{
    [EditorRequired]
    [Parameter]
    public List<AppDto> AppItems { get; set; } = new();

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<ApiPermissionDetailDto> OnSubmit { get; set; }

    [EditorRequired]
    [Parameter]
    public List<SelectItemDto<PermissionTypes>> PermissionTypes { get; set; } = new();

    MForm _form = default!;
    ApiPermissionDetailDto _apiPermissionDetailDto = new();

    private async Task OnSubmitHandler()
    {
        if (await _form.ValidateAsync())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_apiPermissionDetailDto);
            }
        }
    }
}
