namespace Masa.Auth.Admin.Pages.Subjects.Users;

public partial class User
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
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageCount { get; set; }

    public long TotalCount { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<UserItemResponse> Users { get; set; } = new();

    public UserItemResponse CurrentUser { get; set; } = UserItemResponse.Default;

    public List<DataTableHeader<UserItemResponse>> Headers { get; set; } = new();

    public bool UserDialogVisible { get; set; }

    public bool AuthorizeDialogVisible { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(UserItemResponse.Avatar)), Value = nameof(UserItemResponse.Avatar), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.DisplayName)), Value = nameof(UserItemResponse.DisplayName), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.Email)), Value = nameof(UserItemResponse.Email), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.PhoneNumber)), Value = nameof(UserItemResponse.PhoneNumber), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.CreationTime)), Value = nameof(UserItemResponse.CreationTime), Sortable = false },
            new() { Text = T(nameof(UserItemResponse.Enabled)), Value = T("State"), Sortable = false },
            new() { Text = T("Action"), Value = T("Action"), Sortable = false },
        };

        await GetUserItemsAsync();
    }

    public async Task GetUserItemsAsync()
    {
        Lodding = true;
        var request = new GetUserItemsRequest(PageIndex,PageSize,Search,Enabled);
        var reponse = await AuthClient.GetUserItemsAsync(request);
        if (reponse.Success)
        {
            Users = reponse.Data ?? new();
        }
        else OpenErrorMessage(T("Failed to query user data !"));
        Lodding = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentUser = UserItemResponse.Default;
        UserDialogVisible = true;
    }

    public void OpenEditUserDialog(UserItemResponse user)
    {
        CurrentUser = user;
        UserDialogVisible = true;
    }

    public void OpenAuthorizeDialog(UserItemResponse user)
    {
        CurrentUser = user;
        AuthorizeDialogVisible = true;
    }
}

