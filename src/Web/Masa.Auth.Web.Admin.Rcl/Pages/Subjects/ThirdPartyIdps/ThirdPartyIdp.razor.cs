namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.ThirdPartyIdps;

public partial class ThirdPartyIdp
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
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetThirdPartyIdpsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyIdpDto> ThirdPartyIdps { get; set; } = new();

    public Guid CurrentThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyIdpDto>> Headers { get; set; } = new();

    public bool ThirdPartyIdpDialogVisible { get; set; }

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("Platform"), Value = nameof(ThirdPartyIdpDto.Icon), Sortable = false },
            new() { Text = T("ThirdPartyIdp.Name"), Value = nameof(ThirdPartyIdpDto.Name), Sortable = false },
            new() { Text = T("ThirdPartyIdp.DisplayName"), Value = nameof(ThirdPartyIdpDto.DisplayName), Sortable = false },
            new() { Text = T("Type"), Value = nameof(ThirdPartyIdpDto.AuthenticationType), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpDto.CreationTime)), Value = nameof(ThirdPartyIdpDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyIdpDto.Url)), Value = nameof(ThirdPartyIdpDto.Url), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetThirdPartyIdpsAsync();
    }

    public async Task GetThirdPartyIdpsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyIdpIsDto(Page, PageSize, Search);
        var response = await ThirdPartyIdpService.GetThirdPartyIdpsAsync(request);
        ThirdPartyIdps = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentThirdPartyIdpId = Guid.Empty;
        ThirdPartyIdpDialogVisible = true;
    }

    public void OpenEditUserDialog(ThirdPartyIdpDto thirdPartyIdp)
    {
        CurrentThirdPartyIdpId = thirdPartyIdp.Id;
        ThirdPartyIdpDialogVisible = true;
    }

    public async Task OpenDeteteThirdPartyIdpDialog(ThirdPartyIdpDto thirdPartyIdp)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete data"));
        if (confirm) await DeleteThirdPartyIdpAsync();
    }

    public async Task DeleteThirdPartyIdpAsync()
    {
        Loading = true;
        await ThirdPartyIdpService.DeleteThirdPartyIdpAsync(CurrentThirdPartyIdpId);
        OpenSuccessMessage(T("Success to delete thirdPartyIdp"));
        Loading = false;
    }
}

