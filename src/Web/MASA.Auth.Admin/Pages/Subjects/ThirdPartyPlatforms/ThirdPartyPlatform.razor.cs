namespace Masa.Auth.Admin.Pages.Subjects.ThirdPartyPlatforms;

public partial class ThirdPartyPlatform
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
            GetThirdPartyPlatformItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyPlatformItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetThirdPartyPlatformItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyPlatformItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 }; 

    public List<ThirdPartyPlatformItemResponse> ThirdPartyPlatforms { get; set; } = new();

    public Guid CurrentThirdPartyPlatformId { get; set; }

    public List<DataTableHeader<ThirdPartyPlatformItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyPlatformDialogVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyPlatformItemResponse.Icon), Sortable = false },
            new() { Text = T("ThirdPartyPlatform.Name"), Value = nameof(ThirdPartyPlatformItemResponse.Name), Sortable = false },
            new() { Text = T("ThirdPartyPlatform.DisplayName"), Value = nameof(ThirdPartyPlatformItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyPlatformItemResponse.VerifyType), Sortable = false },
            new() { Text = T(nameof(ThirdPartyPlatformItemResponse.CreationTime)), Value = nameof(ThirdPartyPlatformItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyPlatformItemResponse.Url)), Value = nameof(ThirdPartyPlatformItemResponse.Url), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetThirdPartyPlatformItemsAsync();
    }

    public async Task GetThirdPartyPlatformItemsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyPlatformItemsRequest(PageIndex, PageSize, Search);
        var reponse = await AuthClient.GetThirdPartyPlatformItemsAsync(request);
        if (reponse.Success)
        {
            ThirdPartyPlatforms = reponse.Data;
        }
        else OpenErrorMessage(T("Failed to query thirdPartyPlatform data !"));
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentThirdPartyPlatformId = Guid.Empty;
        ThirdPartyPlatformDialogVisible = true;
    }

    public void OpenEditUserDialog(ThirdPartyPlatformItemResponse thirdPartyPlatform)
    {
        CurrentThirdPartyPlatformId = thirdPartyPlatform.ThirdPartyPlatformId;
        ThirdPartyPlatformDialogVisible = true;
    }

    public void OpenDeteteThirdPartyPlatformDialog(ThirdPartyPlatformItemResponse thirdPartyPlatform)
    {
        CurrentThirdPartyPlatformId = thirdPartyPlatform.ThirdPartyPlatformId;
        OpenConfirmDialog(async confirm => 
        {
            if (confirm) await DeleteThirdPartyPlatformAsync();
        },
        "Are you sure delete data");
    }

    public async Task DeleteThirdPartyPlatformAsync()
    {
        Loading = true;
        var reponse = await AuthClient.DeleteThirdPartyPlatformAsync(CurrentThirdPartyPlatformId);
        if (reponse.Success)
        {
            OpenSuccessMessage(T("Success to delete thirdPartyPlatform !"));
        }
        else OpenErrorMessage(T("Failed to delete thirdPartyPlatform !"));
        Loading = false;
    }
}

