namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class ThirdPartyIdp
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
            GetThirdPartyIdpItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyIdpItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetThirdPartyIdpItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyIdpItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyIdpItemResponse> ThirdPartyIdps { get; set; } = new();

    public Guid CurrentThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyIdpItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyIdpDialogVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyIdpItemResponse.Icon), Sortable = false },
            new() { Text = T("ThirdPartyIdp.Name"), Value = nameof(ThirdPartyIdpItemResponse.Name), Sortable = false },
            new() { Text = T("ThirdPartyIdp.DisplayName"), Value = nameof(ThirdPartyIdpItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyIdpItemResponse.VerifyType), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpItemResponse.CreationTime)), Value = nameof(ThirdPartyIdpItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpItemResponse.Url)), Value = nameof(ThirdPartyIdpItemResponse.Url), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetThirdPartyIdpItemsAsync();
    }

    public async Task GetThirdPartyIdpItemsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyIdpItemsRequest(PageIndex, PageSize, Search);
        var response = await AuthClient.GetThirdPartyIdpItemsAsync(request);
        if (response.Success)
        {
            ThirdPartyIdps = response.Data;
        }
        else OpenErrorMessage(T("Failed to query thirdPartyIdpList data:") + response.Message);
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentThirdPartyIdpId = Guid.Empty;
        ThirdPartyIdpDialogVisible = true;
    }

    public void OpenEditUserDialog(ThirdPartyIdpItemResponse thirdPartyIdp)
    {
        CurrentThirdPartyIdpId = thirdPartyIdp.ThirdPartyIdpId;
        ThirdPartyIdpDialogVisible = true;
    }

    public void OpenDeteteThirdPartyIdpDialog(ThirdPartyIdpItemResponse thirdPartyIdp)
    {
        CurrentThirdPartyIdpId = thirdPartyIdp.ThirdPartyIdpId;
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await DeleteThirdPartyIdpAsync();
        },
        T("Are you sure delete data?"));
    }

    public async Task DeleteThirdPartyIdpAsync()
    {
        Loading = true;
        var response = await AuthClient.DeleteThirdPartyIdpAsync(CurrentThirdPartyIdpId);
        if (response.Success)
        {
            OpenSuccessMessage(T("Success to delete thirdPartyIdp"));
        }
        else OpenErrorMessage(T("Failed to delete thirdPartyIdp:") + response.Message);
        Loading = false;
    }
}

