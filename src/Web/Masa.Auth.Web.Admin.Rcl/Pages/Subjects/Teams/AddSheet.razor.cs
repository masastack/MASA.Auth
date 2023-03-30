// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddSheet
{
    [Parameter]
    public EventCallback<TeamDetailDto> OnSubmit { get; set; }

    TeamDetailDto _teamDetailDto = new TeamDetailDto();
    int _step = 1;
    bool _visible;

    protected override void OnInitialized()
    {
        PageName = "TeamBlock";
        base.OnInitialized();
    }

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(_teamDetailDto);
        }
        _visible = false;
    }

    public void Show()
    {
        _visible = true;
        _teamDetailDto = new();
        _step = 1;
    }
}
