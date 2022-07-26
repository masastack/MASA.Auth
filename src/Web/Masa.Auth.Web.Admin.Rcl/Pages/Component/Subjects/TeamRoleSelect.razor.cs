// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class TeamRoleSelect
{
    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<List<Guid>> RolesChanged { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    private List<TeamRoleSelectDto> Teams { get; set; } = new();

    private TeamService TeamService => AuthCaller.TeamService;

    protected override async Task OnInitializedAsync()
    {
        Teams = await TeamService.GetTeamRoleSelectAsync();
    }

    public async Task UpdateValueAsync(List<Guid> teams)
    {
        var roles = Teams.Where(team => teams.Contains(team.Id))
                                 .SelectMany(team => team.Roles)
                                 .ToList();
        var minLimitRole = roles.OrderBy(role => role.AvailableQuantity)
                                .FirstOrDefault();
        if (minLimitRole is not null && minLimitRole.AvailableQuantity <= 0)
        {
            var team = Teams.First(team => team.Roles.Contains(minLimitRole));
            OpenErrorMessage(string.Format(T("Due to the role [{0}] limit constraint, Team [{1}] cannot add members"), minLimitRole.Name, team.Name));
            teams.Remove(team.Id);
        }
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(teams);
        else Value = Value;
        if(RolesChanged.HasDelegate)
        {
            await RolesChanged.InvokeAsync(roles.Select(role => role.Id).ToList());
        }
    }
}

