// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Logs;

public partial class OperationLog
{
    private string? _search;
    private int _page = 1;
    private int _pageSize = 10;
    private Guid _userId;
    private DateOnly? _startTime;
    private DateOnly? _endTime = DateOnly.FromDateTime(DateTime.Now);

    public Guid UserId
    {
        get { return _userId; }
        set
        {
            _userId = value;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
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
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align="center", Width="105px" },
    };

    public async Task GetOperationLogsAsync()
    {
        Loading = true;
        var reuquest = new GetOperationLogsDto(Page, PageSize, UserId, StartTime?.ToDateTime(TimeOnly.MinValue), EndTime?.ToDateTime(TimeOnly.MaxValue), Search);
        var response = await OperationLogService.GetListAsync(reuquest);
        OperationLogs = response.Items;
        Total = response.Total;
    }
}
