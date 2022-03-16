using Masa.Auth.ApiGateways.Caller.Response.Subjects;

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

    public List<ThirdPartyUserItemResponse> ThirdPartyUsers { get; set; } = new();

    public Guid CurrentThirdPartyUserId { get; set; }

    public List<ThirdPartyIdpItemResponse> ThirdPartyIdps { get; set; } = new();

    public Guid ThirdPartyIdpId { get; set; }

    public List<DataTableHeader<ThirdPartyUserItemResponse>> Headers { get; set; } = new();

    public bool ThirdPartyUserDialog { get; set; }

    private ThirdPartyUserService ThirdPartyUserService => AuthCaller.ThirdPartyUserService;

    private ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserItemResponse.Avatar)), Value = nameof(UserItemResponse.Avatar), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.DisplayName)), Value = nameof(UserItemResponse.DisplayName), Sortable = false },
            new() { Text = T("Source"), Value = nameof(ThirdPartyUserItemResponse.ThirdPartyIdpId), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.CreationTime)), Value = nameof(ThirdPartyUserItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.Creator)), Value = nameof(ThirdPartyUserItemResponse.Creator), Sortable = false },
            new() { Text = T(nameof(ThirdPartyUserItemResponse.ModificationTime)), Value = nameof(ThirdPartyUserItemResponse.ModificationTime), Sortable = false },
            new() { Text = T("State"), Value = T(nameof(UserItemResponse.Enabled)), Sortable = false },
        };

        await GetThirdPartyUserItemsAsync();
        await SelectThirdPartyIdpAsync();
    }

    public async Task GetThirdPartyUserItemsAsync()
    {
        Loading = true;
        var request = new GetThirdPartyUserItemsRequest(PageIndex, PageSize, Search, Enabled, ThirdPartyIdpId);
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

    public void OpenEditThirdPartyUserDialog(ThirdPartyUserItemResponse thirdPartyUser)
    {
        CurrentThirdPartyUserId = thirdPartyUser.ThirdPartyUserId;
        ThirdPartyUserDialog = true;
    }
}

