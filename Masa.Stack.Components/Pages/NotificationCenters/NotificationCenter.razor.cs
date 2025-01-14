// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Stack.Components.NotificationCenters;

namespace Masa.Stack.Components;

public partial class NotificationCenter : MasaComponentBase
{
    [Inject]
    public NoticeState NoticeState { get; set; } = default!;

    [Parameter]
    public string MessageId { get; set; }

    private NotificationLeft _messageLeftRef = default!;
    private NotificationRight _messageRightRef = default!;
    private bool _detailShow = false;
    private Guid _messageId;
    private ChannelModel? _channel;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(MessageId))
        {
            _detailShow = false;
            _messageId = default(Guid);
        }
        else
        {
            _messageId = Guid.Parse(MessageId);
            _detailShow = true;
        }
    }

    private void HandleListItemClick(Guid messageId)
    {
        _messageId = messageId;
        _detailShow = true;
    }

    private void HandleDetailBack()
    {
        _detailShow = false;
    }

    private async void HandleChannelClick(ChannelModel? channel)
    {
        _channel = channel;
        _detailShow = false;
    }

    private async Task HandleAllRead()
    {
        await _messageLeftRef.LoadData();
    }

    private async Task HandleOnOk()
    {
        await _messageLeftRef.LoadData();
        await _messageRightRef.LoadData();
        await LoadNotices();
    }

    private async Task LoadNotices()
    {
        GetNoticeListModel _queryParam = new();
        var dtos = await McClient.WebsiteMessageService.GetNoticeListAsync(_queryParam);
        NoticeState.SetNotices(dtos);
    }
}
