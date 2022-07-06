// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Logs;

public partial class ViewOperationLogDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public Guid OperationLogId { get; set; }

    private OperationLogDetailDto OperationLogDetail { get; set; } = new();

    private OperationLogService OperationLogService => AuthCaller.OperationLogService;

    protected override string? PageName { get; set; } = "OperationLogBlock";

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetOperationLogDetailAsync();
        }
    }

    public async Task GetOperationLogDetailAsync()
    {
        OperationLogDetail = await OperationLogService.GetDetailAsync(OperationLogId);
    }
}

