namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class User
{
    private string? _name;
    private string? _email;
    private string? _phoneNumber;
    private bool? _enabled;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _name ?? ""; }
        set
        {
            _name = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string Email
    {
        get { return _email ?? ""; }
        set
        {
            _email = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string PhoneNumber
    {
        get { return _phoneNumber ?? ""; }
        set
        {
            _phoneNumber = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetUserAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Filter { get; set; }

    public long Total { get; set; }

    public List<UserDto> Users { get; set; } = new();

    public Guid CurrentUserId { get; set; }

    public List<DataTableHeader<UserDto>> Headers { get; set; } = new();

    public List<(string, bool?)> UserStateSelect { get; set; } = new();

    public bool AddUserDialogVisible { get; set; }

    public bool UpdateUserDialogVisible { get; set; }

    public bool AuthorizeDialogVisible { get; set; }

    private UserService UserService => AuthCaller.UserService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("User"), Value = nameof(UserDto.Avatar), Sortable = false },
            new() { Text = T(nameof(UserDto.Account)), Value = nameof(UserDto.Account), Sortable = false },
            new() { Text = T(nameof(UserDto.PhoneNumber)), Value = nameof(UserDto.PhoneNumber), Sortable = false },
            new() { Text = T(nameof(UserDto.CreationTime)), Value = nameof(UserDto.CreationTime), Sortable = false },
            new() { Text = T("State"), Value = nameof(UserDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        UserStateSelect = new()
        {
            (@T("Enable"), true),
            (@T("Disabled"), false),
        };

        await GetUserAsync();
    }

    public async Task GetUserAsync()
    {
        Loading = true;
        var request = new GetUsersDto(Page, PageSize, default, Enabled);
        var response = await UserService.GetListAsync(request);
        Users = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddUserDialog()
    {
        AddUserDialogVisible = true;
    }

    public void OpenUpdateUserDialog(UserDto user)
    {
        CurrentUserId = user.Id;
        UpdateUserDialogVisible = true;
    }

    public void OpenAuthorizeDialog(UserDto user)
    {
        CurrentUserId = user.Id;
        AuthorizeDialogVisible = true;
    }
}

