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

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyIdpItemResponse> ThirdPartyIdps { get; set; } = new();

    public Guid CurrentThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyIdpItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyIdpDialogVisible { get; set; }

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyIdpItemResponse.Icon), Sortable = false },
            new() { Text = T("ThirdPartyIdp.Name"), Value = nameof(ThirdPartyIdpItemResponse.Name), Sortable = false },
            new() { Text = T("ThirdPartyIdp.DisplayName"), Value = nameof(ThirdPartyIdpItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyIdpItemResponse.AuthenticationType), Sortable = false },
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
        var response = await ThirdPartyIdpService.GetThirdPartyIdpItemsAsync(request);
        ThirdPartyIdps = response.Items;
        TotalPages = response.TotalPages;
        Total = response.Total;
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
        await ThirdPartyIdpService.DeleteThirdPartyIdpAsync(CurrentThirdPartyIdpId);
        OpenSuccessMessage(T("Success to delete thirdPartyIdp"));
        Loading = false;
    }
}

