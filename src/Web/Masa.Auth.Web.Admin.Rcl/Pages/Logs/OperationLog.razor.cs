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
    private DateTimeOffset? _startTime;
    private DateTimeOffset? _endTime;
    private OperationTypes _operationType;

    public Guid UserId
    {
        get => _userId;
        set
        {
            var temp = _userId;
            if (temp != value)
            {
                _userId = value;
                _page = 1;
                OnParameterChangedAsync();
            }
        }
    }

    public OperationTypes OperationType
    {
        get => _operationType;
        set
        {
            var temp = _operationType;
            if (temp != value)
            {
                _operationType = value;
                _page = 1;
                OnParameterChangedAsync();
            }
        }
    }

    public string OperationTypeDisplay(OperationTypes operationType)
    {
        return T(operationType.ToString());
    }

    private Task OnDateTimeUpdate((DateTimeOffset? start, DateTimeOffset? end) arg)
    {
        _startTime = arg.start;
        _endTime = arg.end;
        return OnParameterChangedAsync();
    }

    public string Search
    {
        get => _search ?? "";
        set
        {
            var temp = _search;
            if (temp != value)
            {
                _search = value;
                _page = 1;
                OnParameterChangedAsync();
            }
        }
    }

    public int Page
    {
        get => _page;
        set
        {
            var temp = _page;
            if (temp != value)
            {
                _page = value;
                if (_total != 0 && _page > (_total / PageSize))
                {
                    _page = 1;
                }
                OnParameterChangedAsync();
            }
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            var temp = _page;
            if (temp != value)
            {
                _pageSize = value;
                _page = 1;
                OnParameterChangedAsync();
            }
        }
    }

    private long _total;

    private List<OperationLogDto> _operationLogs = new();

    private OperationLogService OperationLogService => AuthCaller.OperationLogService;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            PageName = "OperationLogBlock";
            await GetOperationLogsAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private Task OnParameterChangedAsync()
    {
        return GetOperationLogsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
    }

    public async Task GetOperationLogsAsync()
    {
        var request = new GetOperationLogsDto(Page, PageSize, UserId, _startTime?.UtcDateTime, _endTime?.UtcDateTime, OperationType, Search);
        var response = await OperationLogService.GetListAsync(request);
        _operationLogs = response.Items;
        _operationLogs.ForEach(operationLog => operationLog.OperationTime = operationLog.OperationTime.Add(JsInitVariables.TimezoneOffset));
    }

}
