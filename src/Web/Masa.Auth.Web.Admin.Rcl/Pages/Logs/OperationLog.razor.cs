// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using JsInitVariables = Masa.Stack.Components.JsInitVariables;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Logs;

public partial class OperationLog
{
    private string? _search;
    private int _page = 1;
    private int _pageSize = 10;
    private Guid _userId;
    private DateTime? _startTime;
    private DateTime? _endTime = DateTime.UtcNow;
    private OperationTypes _operationType;

    public Guid UserId
    {
        get { return _userId; }
        set
        {
            _userId = value;
            _page = 1;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public OperationTypes OperationType
    {
        get { return _operationType; }
        set
        {
            _operationType = value;
            _page = 1;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateTime? StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            _page = 1;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateTime? EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            _page = 1;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _page = 1;
            _pageSize = value;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<OperationLogDto> OperationLogs { get; set; } = new();

    public int CurrentOperationLogId { get; set; }

    public bool ViewOperationLogDialogVisible { get; set; }

    private OperationLogService OperationLogService => AuthCaller.OperationLogService;

    protected override async Task OnInitializedAsync()
    {
        PageName = "OperationLogBlock";
        await GetOperationLogsAsync();
    }

    public List<DataTableHeader<OperationLogDto>> GetHeaders() => new()
    {
        new() { Text = T(nameof(OperationLogDto.OperationTime)), Value = nameof(OperationLogDto.OperationTime), Sortable = false },
        new() { Text = T(nameof(OperationLogDto.OperatorName)), Value = nameof(OperationLogDto.OperatorName), Sortable = false },
        new() { Text = T(nameof(OperationLogDto.OperationType)), Value = nameof(OperationLogDto.OperationType), Sortable = false },
        new() { Text = T(nameof(OperationLogDto.OperationDescription)), Value = nameof(OperationLogDto.OperationDescription), Sortable = false },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align=DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetOperationLogsAsync()
    {
        Loading = true;
        var reuquest = new GetOperationLogsDto(Page, PageSize, UserId, StartTime, EndTime, OperationType, Search);
        var response = await OperationLogService.GetListAsync(reuquest);
        OperationLogs = response.Items;
        OperationLogs.ForEach(operationLog => operationLog.OperationTime = operationLog.OperationTime.Add(JsInitVariables.TimezoneOffset));
        Total = response.Total;
    }
}
