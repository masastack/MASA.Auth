// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Webhooks;

public partial class Index
{
    AddDialog _addDialog = null!;

    GetWebhookPaginationDto _getWebhookPaginationDto = new();
    PaginationDto<WebhookItemDto> _paginationDto = new();

    WebhookService _webhookService => AuthCaller.WebhookService;

    private List<DataTableHeader<WebhookItemDto>> _headers = new(); 

    protected override void OnInitialized()
    {
        PageName = "Webhook";

        _headers = new List<DataTableHeader<WebhookItemDto>> {
            new() { Text = T(nameof(WebhookItemDto.Name)), Value = nameof(WebhookItemDto.Name), Sortable = false , Width="300px"},
            new() { Text = T(nameof(WebhookItemDto.Url)), Value = nameof(WebhookItemDto.Url), Sortable = false, Width="300px" },
            new() { Text = T(nameof(WebhookItemDto.Event)), Value = nameof(WebhookItemDto.Event), Sortable = false, Width="105px" },
            new() { Text = T(nameof(WebhookItemDto.IsActive)), Value = nameof(WebhookItemDto.IsActive), Sortable = false, Width="105px" },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
        };

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadData()
    {
        _paginationDto = await _webhookService.GetListAsync(_getWebhookPaginationDto);
    }

    private async Task OpenUpdateDialog(Guid id)
    {
        await _addDialog.ShowAsync(id);
    }

    private async Task PageChangedHandler(int page)
    {
        _getWebhookPaginationDto.Page = page;
        await LoadData();
    }

    private async Task PageSizeChangedHandler(int pageSize)
    {
        _getWebhookPaginationDto.PageSize = pageSize;
        await LoadData();
    }

    private async Task SearchKeyHandler()
    {
        _getWebhookPaginationDto.Page = 1;
        await LoadData();
    }

    private async Task OpenRemoveDialog(WebhookItemDto webhookItemDto)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Webhook"), T("Are you sure to delete webhook {0}", webhookItemDto.Name));
        if (isConfirmed)
        {
            await _webhookService.RemoveAsync(webhookItemDto.Id);
            await LoadData();
        }
    }
}
