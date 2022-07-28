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
    public List<Guid> Excludes { get; set; } = new();

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

    public async Task UpdateValueAsync(List<Guid> value)
    {
        var newTeams = value.Except(Excludes).ToList();
        var minLimitRole = Teams.Where(team => newTeams.Contains(team.Id))
                                .SelectMany(team => team.Roles)
                                .OrderBy(role => role.AvailableQuantity)
                                .FirstOrDefault();
        if (minLimitRole is not null && minLimitRole.AvailableQuantity <= 0)
        {
            var team = Teams.First(team => team.Roles.Contains(minLimitRole));
            OpenErrorMessage(string.Format(T("Due to the role [{0}] limit constraint, Team [{1}] cannot add members"), minLimitRole.Name, team.Name));
            value.Remove(team.Id);
        }
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
        if (RolesChanged.HasDelegate)
        {
            var roles = Teams.Where(team => value.Contains(team.Id))
                             .SelectMany(team => team.Roles)
                             .Distinct();
            await RolesChanged.InvokeAsync(roles.Select(role => role.Id).ToList());
        }
    }
}

