// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class RoleSelectForRole : RoleSelect
{
    [Parameter]
    public Guid RoleId { get; set; }

    private Guid OldRoleId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Label = T("Inherited Role");
        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RoleId != OldRoleId)
        {
            await ReloadAsync();
        }
    }

    public async Task ReloadAsync()
    {
        OldRoleId = RoleId;
        Roles = await RoleService.GetSelectForRoleAsync(RoleId);
    }
}

