// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public class RoleSelectForRole : RoleSelect
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid _oldRoleId;

    protected override async Task OnInitializedAsync()
    {
        Label = T("Inherited Role");
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RoleId != _oldRoleId)
        {
            await ReloadAsync();
        }
    }

    public async Task ReloadAsync()
    {
        _oldRoleId = RoleId;
        Roles = await RoleService.GetSelectForRoleAsync(RoleId);
    }
}

