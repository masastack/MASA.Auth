namespace Masa.Auth.Admin.Pages.Subjects.Users;

public partial class Staff
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
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetStaffAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<ThirdPartyUserItemResponse> Staffs { get; set; } = new();

    public Guid CurrentStaffId { get; set; }

    public List<DataTableHeader<ThirdPartyUserItemResponse>> Headers { get; set; } = new();

    public bool StaffDialog { get; set; }

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

        await GetStaffAsync();
    }

    public async Task GetStaffAsync()
    {
        //Lodding = true;
        //var request = new GetThirdPartyUserItemsRequest(PageIndex, PageSize, Search, Enabled, ThirdPartyPlatformId);
        //var reponse = await AuthClient.GetThirdPartyUserItemsAsync(request);
        //if (reponse.Success)
        //{
        //    Staffs = reponse.Data;
        //}
        //else OpenErrorMessage(T("Failed to query thirdPartyUser data !"));
        //Lodding = false;
        await Task.CompletedTask;
    }


    public void OpenEditThirdPartyUserDialog(ThirdPartyUserItemResponse thirdPartyUser)
    {
        CurrentStaffId = thirdPartyUser.ThirdPartyUserId;
        StaffDialog = true;
    }
}

