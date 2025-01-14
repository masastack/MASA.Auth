// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components.NotificationCenters;

public partial class NotificationRight : MasaComponentBase
{
    [Inject]
    public JsInitVariables JsInitVariables { get; set; } = default!;

    [Inject]
    public NoticeState NoticeState { get; set; } = default!;

    [Parameter]
    public ChannelModel? Channel { get; set; }

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<Guid> OnItemClick { get; set; }

    [Parameter]
    public EventCallback OnAllRead { get; set; }

    private GetWebsiteMessageModel _queryParam = new(1, 10);
    private PaginatedListModel<WebsiteMessageModel> _entities = new();
    private List<WebsiteMessageFilterType> _filterTypeItems = Enum.GetValues(typeof(WebsiteMessageFilterType)).Cast<WebsiteMessageFilterType>().ToList();

    protected override async Task OnParametersSetAsync()
    {
        _queryParam.ChannelId = Channel?.Id;
        await RefreshAsync();
    }

    protected override void OnInitialized()
    {
        TypeAdapterConfig<GetWebsiteMessageModel, ReadAllWebsiteMessageModel>.NewConfig().MapToConstructor(true);
    }

    public async Task LoadData()
    {
        _entities = (await McClient.WebsiteMessageService.GetListAsync(_queryParam));
    }

    private async Task HandleOnClick(WebsiteMessageModel item)
    {
        if (!string.IsNullOrEmpty(item.LinkUrl))
        {
            NavigationManager.NavigateTo(item.LinkUrl);
            return;
        }

        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.InvokeAsync(item.Id);
        }
    }

    public async Task RefreshAsync()
    {
        _queryParam.Page = 1;
        await LoadData();
    }

    private async Task HandlePageChanged(int page)
    {
        _queryParam.Page = page;
        await LoadData();
    }

    private async Task HandlePageSizeChanged(int pageSize)
    {
        _queryParam.PageSize = pageSize;
        await LoadData();
    }

    private async Task HandleShowAll()
    {
        _queryParam.IsRead = null;
        await RefreshAsync();
    }

    private async Task HandleShowUnread()
    {
        _queryParam.IsRead = false;
        await RefreshAsync();
    }

    private async Task HandleMarkAllRead()
    {
        var dto = _queryParam.Adapt<ReadAllWebsiteMessageModel>();
        await McClient.WebsiteMessageService.SetAllReadAsync(dto);
        await PopupService.EnqueueSnackbarAsync(T("OperationSuccessfulMessage"), AlertTypes.Success);
        await LoadData();
        NoticeState.SetAllRead();
        if (OnAllRead.HasDelegate)
        {
            await OnAllRead.InvokeAsync();
        }
    }
}
