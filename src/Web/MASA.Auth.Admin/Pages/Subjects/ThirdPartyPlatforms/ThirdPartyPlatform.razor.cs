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

    public List<ThirdPartyIdpItemResponse> ThirdPartyPlatforms { get; set; } = new();

    public Guid CurrentThirdPartyPlatformId { get; set; }

    public List<DataTableHeader<ThirdPartyIdpItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyPlatformDialogVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyIdpItemResponse.Icon), Sortable = false },
            new() { Text = T("ThirdPartyPlatform.Name"), Value = nameof(ThirdPartyIdpItemResponse.Name), Sortable = false },
            new() { Text = T("ThirdPartyPlatform.DisplayName"), Value = nameof(ThirdPartyIdpItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyIdpItemResponse.VerifyType), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpItemResponse.CreationTime)), Value = nameof(ThirdPartyIdpItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpItemResponse.Url)), Value = nameof(ThirdPartyIdpItemResponse.Url), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetThirdPartyPlatformItemsAsync();
    }

    public async Task GetThirdPartyPlatformItemsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyIdpItemsRequest(PageIndex, PageSize, Search);
        var response = await AuthClient.GetThirdPartyPlatformItemsAsync(request);
        if (response.Success)
        {
            ThirdPartyPlatforms = response.Data;
        }
        else OpenErrorMessage(T("Failed to query thirdPartyPlatformList data:") + response.Message);
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentThirdPartyPlatformId = Guid.Empty;
        ThirdPartyPlatformDialogVisible = true;
    }

    public void OpenEditUserDialog(ThirdPartyIdpItemResponse thirdPartyPlatform)
    {
        CurrentThirdPartyPlatformId = thirdPartyPlatform.ThirdPartyPlatformId;
        ThirdPartyPlatformDialogVisible = true;
    }

    public void OpenDeteteThirdPartyPlatformDialog(ThirdPartyIdpItemResponse thirdPartyPlatform)
    {
        CurrentThirdPartyPlatformId = thirdPartyPlatform.ThirdPartyPlatformId;
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await DeleteThirdPartyPlatformAsync();
        },
        T("Are you sure delete data?"));
    }

    public async Task DeleteThirdPartyPlatformAsync()
    {
        Loading = true;
        var response = await AuthClient.DeleteThirdPartyPlatformAsync(CurrentThirdPartyPlatformId);
        if (response.Success)
        {
            OpenSuccessMessage(T("Success to delete thirdPartyPlatform"));
        }
        else OpenErrorMessage(T("Failed to delete thirdPartyPlatform:") + response.Message);
        Loading = false;
    }
}

