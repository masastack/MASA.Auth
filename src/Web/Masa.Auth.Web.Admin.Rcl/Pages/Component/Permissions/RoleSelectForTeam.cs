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
                        RoleLimitChanged.InvokeAsync(new (minLimitRole.Name, minLimitRole.AvailableQuantity));
                    }
                    else RoleLimitChanged.InvokeAsync(new ("", int.MaxValue));
                }
            }
        }
    }

    [Parameter]
    public int TeamUserCount { get; set; }

    [Parameter]
    public EventCallback<RoleLimitModel> RoleLimitChanged { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ReloadAsync();
    }

    protected override bool RoleDisabled(RoleSelectDto role) => role.Limit != 0 && role.AvailableQuantity < TeamUserCount;

    public async Task ReloadAsync()
    {
        Roles = await RoleService.GetSelectForTeamAsync();
    }
}

