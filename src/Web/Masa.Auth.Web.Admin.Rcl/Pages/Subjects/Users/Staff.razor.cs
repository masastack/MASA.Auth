﻿using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class Staff
{
    private string? _search;
    private bool _enabled;
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<StaffItemResponse> Staffs { get; set; } = new();

    public Guid CurrentStaffId { get; set; }

    public List<DataTableHeader<StaffItemResponse>> Headers { get; set; } = new();

    public bool StaffDialog { get; set; }

    private StaffService StaffService => AuthCaller.StaffService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserItemResponse.Avatar)), Value = nameof(UserItemResponse.Avatar), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.Name)), Value = nameof(UserItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Department"), Value = nameof(StaffItemResponse.Department), Sortable = false },
            new() { Text = T(nameof(StaffItemResponse.JobNumber)), Value = nameof(StaffItemResponse.JobNumber), Sortable = false },
            new() { Text = T(nameof(StaffItemResponse.Position)), Value = nameof(StaffItemResponse.Position), Sortable = false },
            new() { Text = T("State"), Value = nameof(StaffItemResponse.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetStaffAsync();
    }

    public async Task GetStaffAsync()
    {
        Loading = true;
        var request = new GetStaffItemsRequest(PageIndex, PageSize, Search, Enabled);
        var response = await StaffService.GetStaffItemsAsync(request);
        Staffs = response.Items;
        TotalPages = response.TotalPages;
        Total = response.Total;
        Loading = false;
    }

    public void OpenStaffDialog(StaffItemResponse staff)
    {
        CurrentStaffId = staff.StaffId;
        StaffDialog = true;
    }
}

