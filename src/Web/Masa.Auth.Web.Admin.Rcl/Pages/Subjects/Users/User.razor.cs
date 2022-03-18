namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class User
{
    private string? _name;
    private string? _email;
    private string? _phoneNumber;
    private bool _enabled;
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public string Name
    {
        get { return _name ?? ""; }
        set
        {
            _name = value;
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string Email
    {
        get { return _email ?? ""; }
        set
        {
            _email = value;
            GetUserItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string PhoneNumber
    {
        get { return _phoneNumber ?? ""; }
        set
        {
            _phoneNumber = value;
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

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<UserDto> Users { get; set; } = new();

    public Guid CurrentUserId { get; set; }

    public List<DataTableHeader<UserDto>> Headers { get; set; } = new();

    public bool UserDialogVisible { get; set; }

    public bool AuthorizeDialogVisible { get; set; }

    private UserService UserService => AuthCaller.UserService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("User"), Value = nameof(UserDto.Avatar), Sortable = false },
            new() { Text = T(nameof(UserDto.Email)), Value = nameof(UserDto.Email), Sortable = false },
            new() { Text = T(nameof(UserDto.PhoneNumber)), Value = nameof(UserDto.PhoneNumber), Sortable = false },
            new() { Text = T(nameof(UserDto.CreationTime)), Value = nameof(UserDto.CreationTime), Sortable = false },
            new() { Text = T("State"), Value = nameof(UserDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetUserItemsAsync();
    }

    public async Task GetUserItemsAsync()
    {
        Loading = true;
        var request = new GetUsersDto(PageIndex, PageSize, Name, PhoneNumber, Email, Enabled);
        var response = await UserService.GetUserItemsAsync(request);
        Users = response.Items;
        TotalPages = response.TotalPages;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        CurrentUserId = Guid.Empty;
        UserDialogVisible = true;
    }

    public void OpenUpdateUserDialog(UserDto user)
    {
        CurrentUserId = user.UserId;
        UserDialogVisible = true;
    }

    public void OpenAuthorizeDialog(UserDto user)
    {
        CurrentUserId = user.UserId;
        AuthorizeDialogVisible = true;
    }
}

