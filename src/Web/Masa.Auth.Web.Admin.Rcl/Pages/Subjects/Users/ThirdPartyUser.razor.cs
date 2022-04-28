namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ThirdPartyUser
{
    private bool? _enabled;
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
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? StartTime
    {
        get => _startTime;
        set
        {
            _startTime = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public DateOnly? EndTime
    {
        get => _endTime;
        set
        {
            _endTime = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
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

    public bool Filter { get; set; }

    public long Total { get; set; }

    public List<ThirdPartyUserDto> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public List<DataTableHeader<ThirdPartyUserDto>> Headers { get; set; } = new();

    public bool ViewThirdPartyUserDialog { get; set; }

    public bool DomainAccountDialog { get; set; }

    private ThirdPartyUserService ThirdPartyUserService => AuthCaller.ThirdPartyUserService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserDto.Avatar)), Value = nameof(UserDto.Avatar), Sortable = false },
            new() { Text = T("Source"), Value = nameof(ThirdPartyUserDto.ThirdPartyIdp), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.Creator)), Value = nameof(ThirdPartyUserDto.Creator), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.CreationTime)), Value = nameof(ThirdPartyUserDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.ModificationTime)), Value = nameof(ThirdPartyUserDto.ModificationTime), Sortable = false },
            new() { Text = T("State"), Value = T(nameof(UserDto.Enabled)), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        // await GetThirdPartyUsersAsync();
    }

    public async Task GetThirdPartyUsersAsync()
    {
        Loading = true;
        var request = new GetThirdPartyUsersDto(Page, PageSize, UserId, Enabled, StartTime?.ToDateTime(TimeOnly.MinValue), EndTime?.ToDateTime(TimeOnly.MaxValue));
        var response = await ThirdPartyUserService.GetListAsync(request);
        ThirdPartyUsers = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenViewThirdPartyUserDialog(ThirdPartyUserDto thirdPartyUser)
    {
        CurrentThirdPartyUserId = thirdPartyUser.Id;
        ViewThirdPartyUserDialog = true;
    }
}

