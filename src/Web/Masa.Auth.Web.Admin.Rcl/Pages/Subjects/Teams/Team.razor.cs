// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class Team
{
    string _search = string.Empty;
    bool _showEdit;
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
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadTeams()
    {
        _teams = await TeamService.ListAsync(_search);
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
        OpenSuccessMessage(T("Add team data success"));
    }

    private async Task OnUpdateBasicInfo(UpdateTeamBasicInfoDto dto)
    {
        await TeamService.UpdateBasicInfo(dto);
        OpenSuccessMessage(T("Edit team data success"));
        await LoadTeams();
    }

    private async Task OnUpdateAdminPersonnel(UpdateTeamPersonnelDto dto)
    {
        await TeamService.UpdateAdminPersonnel(dto);
        OpenSuccessMessage(T("Edit team data success"));
        await LoadTeams();
    }

    private async Task OnUpdateMemberPersonnel(UpdateTeamPersonnelDto dto)
    {
        await TeamService.UpdateMemberPersonnel(dto);
        OpenSuccessMessage(T("Edit team data success"));
        await LoadTeams();
    }

    private async Task OnDelete(Guid id)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Team"), T("Are you sure you want to delete this team"), AlertTypes.Warning);
        if (isConfirmed)
        {
            await TeamService.DeleteAsync(id);
            await LoadTeams();
        }
    }
}

