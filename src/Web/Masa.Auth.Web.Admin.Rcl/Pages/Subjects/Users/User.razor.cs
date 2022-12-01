// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class User
{
    private bool? _enabled;
    private int _page = 1;
    private int _pageSize = 10;
    private Guid _userId;
    private DateOnly? _startTime;
    private DateOnly? _endTime;

    [Inject]
    public IJSRuntime? Js { get; set; }

    public Guid UserId
    {
        get { return _userId; }
        set
        {
            _userId = value;
            _page = 1;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            _page = 1;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            _page = 1;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            _page = 1;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _page = 1;
            _pageSize = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Filter { get; set; }

    public string FilterClass => Filter is true ? "d-flex show showAnimation" : (Filter is false ? "d-flex close closeAnimation" : "hide");

    public long Total { get; set; }

    public List<UserDto> Users { get; set; } = new();

    public Guid CurrentUserId { get; set; }

    public List<UserSelectDto> UserSelect { get; set; } = new();

    public bool AddUserDialogVisible { get; set; }

    public bool UpdateUserDialogVisible { get; set; }

    public bool AuthorizeDialogVisible { get; set; }

    private UserService UserService => AuthCaller.UserService;

    [Parameter]
    public Staff? StaffCompontent { get; set; }

    [Parameter]
    public ThirdPartyUser? ThirdPartyUserCompontent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PageName = "UserBlock";
        await GetUserAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await using var businessJs = await Js!.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Auth.Web.Admin.Rcl/js/business.js");
            await businessJs.InvokeVoidAsync("onUserFileterAnimationEnd");
        }
    }

    public List<DataTableHeader<UserDto>> GetHeaders() => new()
    {
        new() { Text = T("User"), Value = nameof(UserDto.Avatar), Sortable = false },
        new() { Text = T(nameof(UserDto.Account)), Value = nameof(UserDto.Account), Sortable = false },
        new() { Text = T(nameof(UserDto.CreationTime)), Value = nameof(UserDto.CreationTime), Sortable = false },
        new() { Text = T("State"), Value = nameof(UserDto.Enabled), Sortable = false, Width="105px" },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetUserAsync()
    {
        Loading = true;
        var request = new GetUsersDto(Page, PageSize, UserId, Enabled, StartTime?.ToDateTime(TimeOnly.MinValue), EndTime?.ToDateTime(TimeOnly.MaxValue));
        var response = await UserService.GetListAsync(request);
        Users = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public async Task ReloadAsync()
    {
        await GetUserAsync();
        await base.InvokeAsync(StateHasChanged);
    }

    public void OpenAddUserDialog()
    {
        AddUserDialogVisible = true;
    }

    public void OpenUpdateUserDialog(UserDto user)
    {
        CurrentUserId = user.Id;
        UpdateUserDialogVisible = true;
    }

    public void OpenAuthorizeDialog(UserDto user)
    {
        CurrentUserId = user.Id;
        AuthorizeDialogVisible = true;
    }

    async Task UpdateStaffAndThirdPartyUserAsync()
    {
        await GetUserAsync();
        if (StaffCompontent is not null)
        {
            await StaffCompontent.ReloadAsync();
        }
        if (ThirdPartyUserCompontent is not null)
        {
            await ThirdPartyUserCompontent.ReloadAsync();
        }
    }

    async Task OnAddUserSuccessAsync()
    {
        _enabled = default;
        _page = 1;
        _userId = default;
        _startTime = default;
        _endTime = default;
        await GetUserAsync();
    }
}

