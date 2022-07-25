// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class RoleSelectForTeam : RoleSelect
{
    [Parameter]
    public int TeamUserCount { get; set; }

    public int Limit => Roles.Where(r => Value.Contains(r.Id) && r.Limit != 0).Any() ? Roles.Where(r => Value.Contains(r.Id) && r.Limit != 0).Min(r => r.AvailableQuantity) : int.MaxValue;

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity <= TeamUserCount;

    public async Task ReloadAsync()
    {
        Roles = await RoleService.GetSelectForTeamAsync();
    }
}

