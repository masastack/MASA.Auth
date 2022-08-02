// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class TeamSwitch
{
    [Parameter]
    public Guid Value { get; set; } = new();

    [Parameter]
    public EventCallback<Guid> ValueChanged { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    [Parameter]
    public EventCallback<List<Guid>> RolesChanged { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    private List<TeamRoleSelectDto> Teams { get; set; } = new();

    private TeamRoleSelectDto? CurrentTeam => Teams.FirstOrDefault(team => team.Id == Value);

    private TeamService TeamService => AuthCaller.TeamService;

    protected override async Task OnInitializedAsync()
    {
        Teams = await TeamService.GetTeamRoleSelectAsync("", UserId);
    }

    public async Task UpdateValueAsync(Guid value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
        if (RolesChanged.HasDelegate)
        {
            var roles = Teams.FirstOrDefault(team => value == team.Id)?.Roles?.Distinct() ?? new List<RoleSelectDto>();
            await RolesChanged.InvokeAsync(roles.Select(role => role.Id).ToList());
        }
    }
}
