// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components.NotificationCenters;

public partial class NotificationLeft : MasaComponentBase
{
    [Parameter]
    public EventCallback<ChannelModel?> OnItemClick { get; set; }

    private List<WebsiteMessageChannelModel> _channels = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadData()
    {
        _channels = (await McClient.WebsiteMessageService.GetChannelListAsync());
    }

    private async Task HandleOnClick(WebsiteMessageChannelModel? channel)
    {
        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.InvokeAsync(channel?.Channel);
        }
    }
}
