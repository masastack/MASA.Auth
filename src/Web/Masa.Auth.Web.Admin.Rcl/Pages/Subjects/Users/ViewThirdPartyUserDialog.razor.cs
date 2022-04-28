﻿namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ViewThirdPartyUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public Guid ThirdPartyUserId { get; set; }

    private ThirdPartyUserDetailDto ThirdPartyUser { get; set; } = new();

    private ThirdPartyUserService ThirdPartyUserService => AuthCaller.ThirdPartyUserService;

    private async Task UpdateVisible(bool visible)
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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            ThirdPartyUser = await ThirdPartyUserService.GetDetailAsync(ThirdPartyUserId);
        }
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

