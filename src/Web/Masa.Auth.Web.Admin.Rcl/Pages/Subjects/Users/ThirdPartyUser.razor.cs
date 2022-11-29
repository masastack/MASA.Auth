// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ThirdPartyUser
{
    private bool? _enabled;
    private int _page = 1;
    private int _pageSize = 10;
    private string? _search;
    private Guid _thirdPartyId;
    private DateOnly? _startTime;
    private DateOnly? _endTime = DateOnly.FromDateTime(DateTime.Now);
    private LdapDialog ldapDialog = null!;

    [Inject]
    public IJSRuntime? Js { get; set; }

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public Guid ThirdPartyId
    {
        get => _thirdPartyId;
        set
        {
            _thirdPartyId = value;
            _page = 1;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            _page = 1;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            _page = 1;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            _page = 1;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = 1;
            _page = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Filter { get; set; }

    public long Total { get; set; }

    public List<ThirdPartyUserDto> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public bool ViewThirdPartyUserDialog { get; set; }

    private ThirdPartyUserService ThirdPartyUserService => AuthCaller.ThirdPartyUserService;

    public string FilterClass => Filter is true ? "d-flex show showAnimation" : (Filter is false ? "d-flex close closeAnimation" : "hide");

    protected override async Task OnInitializedAsync()
    {
        PageName = "ThirdPartyUser";
        await GetThirdPartyUsersAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await using var businessJs = await Js!.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Auth.Web.Admin.Rcl/js/business.js");
            await businessJs.InvokeVoidAsync("onUserFileterAnimationEnd");
        }
    }

    public List<DataTableHeader<ThirdPartyUserDto>> GetHeaders() => new()
    {
        new() { Text = T("User"), Value = nameof(UserDto.Avatar), Sortable = false },
        new() { Text = T("Source"), Value = nameof(ThirdPartyUserDto.IdpDetailDto), Sortable = false },
        new() { Text = T(nameof(ThirdPartyUserDto.Creator)), Value = nameof(ThirdPartyUserDto.Creator), Sortable = false },
        new() { Text = T(nameof(ThirdPartyUserDto.CreationTime)), Value = nameof(ThirdPartyUserDto.CreationTime), Sortable = false },
        new() { Text = T(nameof(ThirdPartyUserDto.ModificationTime)), Value = nameof(ThirdPartyUserDto.ModificationTime), Sortable = false },
        new() { Text = T("State"), Value = nameof(UserDto.Enabled), Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetThirdPartyUsersAsync()
    {
        Loading = true;
        var request = new GetThirdPartyUsersDto(Page, PageSize, Search, ThirdPartyId, Enabled, StartTime?.ToDateTime(TimeOnly.MinValue), EndTime?.ToDateTime(TimeOnly.MaxValue));
        var response = await ThirdPartyUserService.GetListAsync(request);
        ThirdPartyUsers = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public async Task ReloadAsync()
    {
        await GetThirdPartyUsersAsync();
        await base.InvokeAsync(StateHasChanged);
    }

    public void OpenViewThirdPartyUserDialog(ThirdPartyUserDto thirdPartyUser)
    {
        CurrentThirdPartyUserId = thirdPartyUser.Id;
        ViewThirdPartyUserDialog = true;
    }

    public async Task OpenLdapDialog()
    {
        await ldapDialog.OpenAsync();
    }
}

