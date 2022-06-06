// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class RoleSelectForUser : RoleSelect
{
    [Parameter]
    public Guid UserId { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Label = T("Inherited Role");
        await base.SetParametersAsync(parameters);
    }

    protected override async Task OnInitializedAsync()
    {
        Roles = await RoleService.GetSelectForUserAsync(UserId);
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity <= 0;
}

