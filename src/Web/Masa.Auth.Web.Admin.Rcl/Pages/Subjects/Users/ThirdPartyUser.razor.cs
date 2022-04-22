namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ThirdPartyUser
{
    private string? _search;
    private bool _enabled;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetThirdPartyUsersAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
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

    public long TotalPages { get; set; }

    public bool Filter { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyUserDto> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public List<ThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public Guid ThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyUserDto>> Headers { get; set; } = new();

    public bool ThirdPartyUserDialog { get; set; }

    public bool LDAPDialog { get; set; }

    private ThirdPartyUserService ThirdPartyUserService => AuthCaller.ThirdPartyUserService;

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserDto.Avatar)), Value = nameof(UserDto.Avatar), Sortable = false },
            new() { Text = T(nameof(UserDto.DisplayName)), Value = nameof(UserDto.DisplayName), Sortable = false },
            new() { Text = T("Source"), Value = nameof(ThirdPartyUserDto.ThirdPartyIdpId), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.CreationTime)), Value = nameof(ThirdPartyUserDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.Creator)), Value = nameof(ThirdPartyUserDto.Creator), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserDto.ModificationTime)), Value = nameof(ThirdPartyUserDto.ModificationTime), Sortable = false },
            new() { Text = T("State"), Value = T(nameof(UserDto.Enabled)), Sortable = false },
        };

        await GetThirdPartyUsersAsync();
        await SelectThirdPartyIdpAsync();
    }

    public async Task GetThirdPartyUsersAsync()
    {
        Loading = true;
        var request = new GetThirdPartyUsersDto(Page, PageSize, Search, Enabled, ThirdPartyIdpId);
        var response = await ThirdPartyUserService.GetThirdPartyUsersAsync(request);
        ThirdPartyUsers = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public async Task SelectThirdPartyIdpAsync()
    {
        Loading = true;
        ThirdPartyIdps = await ThirdPartyIdpService.SelectThirdPartyIdpAsync();
        Loading = false;
    }

    public void OpenEditThirdPartyUserDialog(ThirdPartyUserDto thirdPartyUser)
    {
        CurrentThirdPartyUserId = thirdPartyUser.Id;
        ThirdPartyUserDialog = true;
    }

    public void OpenLDAPDialog()
    {
        LDAPDialog = true;
    }
}

