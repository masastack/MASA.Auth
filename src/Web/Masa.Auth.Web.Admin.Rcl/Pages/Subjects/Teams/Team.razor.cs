// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class Team
{
    string _search = string.Empty;
    TeamDetailDto _editTeamDto = new();
    List<TeamDto> _teams = new();
    AddSheet _addSheet = null!;
    UpdateSheet _updateSheet = null!;

    TeamService TeamService => AuthCaller.TeamService;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadTeams();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadTeams()
    {
        _teams = await TeamService.ListAsync(_search);
        StateHasChanged();
    }

    private async Task SearchKeyDown(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == Keyboards.Enter)
        {
            await LoadTeams();
        }
    }

    private async Task EditTeamHandler(Guid id)
    {
        _editTeamDto = await TeamService.GetAsync(id);
        _updateSheet.Show(_editTeamDto);
    }

    private async Task OnCreate(TeamDetailDto dto)
    {
        await TeamService.CreateAsync(dto);
        await LoadTeams();
        OpenSuccessMessage(T("New team data success"));
    }

    private async Task OnUpdate(UpdateTeamDto dto)
    {
        await TeamService.Update(dto);
        OpenSuccessMessage(T("Edit team data success"));
        await LoadTeams();
    }

    private async Task<bool> OnDelete(Guid id, string name)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Team"), T("Are you sure to delete team {0}", name));
        if (isConfirmed)
        {
            await TeamService.DeleteAsync(id);
            await LoadTeams();
        }
        return isConfirmed;
    }
}

