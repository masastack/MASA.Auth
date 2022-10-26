// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Permissions;

public partial class RoleSelectForTeam : RoleSelect
{
    private List<Guid> _value = new();

    [Parameter]
    public override List<Guid> Value
    {
        get => _value;
        set
        {
            if (_value.Count != value.Count || _value.Except(value).Count() > 0)
            {
                _value = value;
                if (RoleLimitChanged.HasDelegate)
                {
                    var minLimitRole = Roles.Where(role => _value.Contains(role.Id) && role.Limit != 0).OrderBy(role => role.AvailableQuantity).FirstOrDefault();
                    if (minLimitRole != null)
                    {
                        RoleLimitChanged.InvokeAsync(new(minLimitRole.Name, minLimitRole.AvailableQuantity));
                    }
                    else RoleLimitChanged.InvokeAsync(new("", int.MaxValue));
                }
            }
        }
    }

    [Parameter]
    public int TeamUserCount { get; set; }

    [Parameter]
    public List<Guid>? ScopeItems { get; set; }

    [Parameter]
    public EventCallback<RoleLimitModel> RoleLimitChanged { get; set; }

    List<Guid> _scopeItems = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ReloadAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override Task OnParametersSetAsync()
    {
        if (ScopeItems?.SequenceEqual(_scopeItems) == false)
        {
            _scopeItems = ScopeItems;
            Roles = Roles.Where(r => ScopeItems.Contains(r.Id)).ToList();
        }
        return base.OnParametersSetAsync();
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity < TeamUserCount;

    public async Task ReloadAsync()
    {
        Roles = await RoleService.GetSelectForTeamAsync();
    }
}

