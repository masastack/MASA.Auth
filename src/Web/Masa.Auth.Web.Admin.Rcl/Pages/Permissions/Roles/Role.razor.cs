namespace Masa.Auth.Web.Admin.Rcl.Pages.Permissions.Roles;

public partial class Role
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
            GetRoleItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetRoleItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageIndex
    {
        get { return _pageIndex; }
        set
        {
            _pageIndex = value;
            GetRoleItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetRoleItemsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long TotalPages { get; set; }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<RoleDto> Roles { get; set; } = new();

    public Guid CurrentRoleId { get; set; }

    public bool RoleDialogVisible { get; set; }

    public List<DataTableHeader<RoleDto>> Headers { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(RoleDto.Name)), Value = nameof(RoleDto.Name), Sortable = false },
            new() { Text = T("State"), Value = nameof(RoleDto.Enabled), Sortable = false },
            new() { Text = T(nameof(RoleDto.CreationTime)), Value = nameof(RoleDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(RoleDto.Creator)), Value = nameof(RoleDto.Creator), Sortable = false },
            new() { Text = T(nameof(RoleDto.ModificationTime)), Value = nameof(RoleDto.ModificationTime), Sortable = false },
            new() { Text = T(nameof(RoleDto.Modifier)), Value = nameof(RoleDto.Modifier), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetRoleItemsAsync();
    }

    public async Task GetRoleItemsAsync()
    {
        Loading = true;
        var reuquest = new GetRolesDto(PageIndex, PageSize, Search, Enabled);
        var response = await RoleService.GetRoleItemsAsync(reuquest);
        Roles = response.Items;
        TotalPages = response.TotalPages;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddTeamDialog()
    {
        CurrentRoleId = Guid.Empty;
        RoleDialogVisible = true;
    }

    public void OpenEditUserDialog(UserDto user)
    {
        CurrentRoleId = user.UserId;
        RoleDialogVisible = true;
    }

    public void OpenAuthorizeDialog(UserDto user)
    {
        CurrentRoleId = user.UserId;
        RoleDialogVisible = true;
    }
}

