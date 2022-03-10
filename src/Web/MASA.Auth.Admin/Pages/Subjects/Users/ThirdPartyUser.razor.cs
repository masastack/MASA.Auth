namespace Masa.Auth.Admin.Pages.Subjects.Users;

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

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyUserItemResponse> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public List<ThirdPartyPlatformItemResponse> ThirdPartyPlatforms { get; set; } = new();

    public Guid ThirdPartyPlatformId { get; set; }

    public List<DataTableHeader<ThirdPartyUserItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyUserDialog { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserItemResponse.Avatar)), Value = nameof(UserItemResponse.Avatar), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.DisplayName)), Value = nameof(UserItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Source"), Value = nameof(ThirdPartyUserItemResponse.ThirdPartyPlatformId), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.CreationTime)), Value = nameof(ThirdPartyUserItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.Creator)), Value = nameof(ThirdPartyUserItemResponse.Creator), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.ModificationTime)), Value = nameof(ThirdPartyUserItemResponse.ModificationTime), Sortable = false },
            new() { Text = T("State"), Value = T(nameof(UserItemResponse.Enabled)), Sortable = false },
        };

        await GetThirdPartyUserItemsAsync();
        await SelectThirdPartyPlatformAsync();
    }

    public async Task GetThirdPartyUserItemsAsync()
    {
        Lodding = true;
        var request = new GetThirdPartyUserItemsRequest(PageIndex, PageSize, Search, Enabled, ThirdPartyPlatformId);
        var reponse = await AuthClient.GetThirdPartyUserItemsAsync(request);
        if (reponse.Success)
        {
            ThirdPartyUsers = reponse.Data;
        }
        else OpenErrorMessage(T("Failed to query thirdPartyUser data !"));
        Lodding = false;
    }

    public async Task SelectThirdPartyPlatformAsync()
    {
        Lodding = true;
        var response = await AuthClient.SelectThirdPartyPlatformAsync();
        if (response.Success is true)
        {
            ThirdPartyPlatforms = response.Data;
        }
        else OpenErrorMessage(T("Failed to query thirdPartyPlatform data !"));
        Lodding = false;
    }

    public void OpenEditThirdPartyUserDialog(ThirdPartyUserItemResponse thirdPartyUser)
    {
        CurrentThirdPartyUserId = thirdPartyUser.ThirdPartyUserId;
        ThirdPartyUserDialog = true;
    }
}

