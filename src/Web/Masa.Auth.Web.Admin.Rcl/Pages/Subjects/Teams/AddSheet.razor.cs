// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Teams;

public partial class AddSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [Parameter]
    public EventCallback<TeamDetailDto> OnSubmit { get; set; }

    TeamDetailDto _teamDetailDto = new TeamDetailDto();
    int _step = 1;

    public async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(_teamDetailDto);
        }
        await ShowChanged.InvokeAsync(false);
    }
}
