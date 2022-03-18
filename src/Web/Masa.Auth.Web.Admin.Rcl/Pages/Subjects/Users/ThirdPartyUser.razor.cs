namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class ThirdPartyUser
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
            GetThirdPartyUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetThirdPartyUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyUserDto> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public List<ThirdPartyIdpIDto> ThirdPartyIdps { get; set; } = new();

    public Guid ThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyUserDto>> Headers { get; set; } = new();

    public bool ThirdPartyUserDialog { get; set; }

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

        await GetThirdPartyUserItemsAsync();
        await SelectThirdPartyIdpAsync();
    }

    public async Task GetThirdPartyUserItemsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyUsersDto(PageIndex, PageSize, Search, Enabled, ThirdPartyIdpId);
        var response = await ThirdPartyUserService.GetThirdPartyUserItemsAsync(request);
        ThirdPartyUsers = response.Items;
        TotalPages = response.TotalPages;
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
        CurrentThirdPartyUserId = thirdPartyUser.ThirdPartyUserId;
        ThirdPartyUserDialog = true;
    }
}

