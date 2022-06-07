// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class RoleSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public bool Readonly { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    protected List<RoleSelectDto> Roles { get; set; } = new();

    protected RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        Label = T("Role");
        Roles = await RoleService.GetSelectForUserAsync();
    }

    protected virtual bool RoleDisabled(RoleSelectDto role) => false;

    protected void RemoveRole(RoleSelectDto role)
    {
        if (Readonly is false)
        {
            Value.Remove(role.Id);
        }
    }
}

