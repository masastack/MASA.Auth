﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class RoleSelect
{
    [Parameter]
    public virtual List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public string Label { get; set; } = "";

    [Parameter]
    public bool Readonly { get; set; }

    protected List<RoleSelectDto> Roles { get; set; } = new();

    protected RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        Label = T("Role");
        Roles = await RoleService.GetSelectForUserAsync();
        await base.OnInitializedAsync();
    }

    protected virtual bool RoleDisabled(RoleSelectDto role) => false;

    protected async Task RemoveRole(RoleSelectDto role)
    {
        if (Readonly is false)
        {
            var value = new List<Guid>();
            value.AddRange(Value);
            value.Remove(role.Id);
            await ValueChanged.InvokeAsync(value);
        }
    }

    protected virtual async Task UpdateValueAsync(List<Guid> value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}

