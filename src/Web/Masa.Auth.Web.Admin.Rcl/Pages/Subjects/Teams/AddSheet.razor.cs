// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddSheet
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<TeamDetailDto> OnSubmit { get; set; }

    TeamDetailDto _teamDetailDto = new TeamDetailDto();
    int _step = 1;

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(_teamDetailDto);
        }
        await Toggle(false);
    }

    public async Task Show()
    {
        await Toggle(true);
        _teamDetailDto = new();
        _step = 1;
    }

    private async Task Toggle(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    private async Task DialogValueChanged(bool value)
    {
        await Toggle(value);
    }
}
