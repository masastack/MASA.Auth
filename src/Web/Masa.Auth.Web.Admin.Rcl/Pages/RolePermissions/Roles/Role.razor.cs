namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class Role
{
    private string? _search;
    private bool? _enabled;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<int> PageSizes = new() { 10, 25, 50, 100 };

    public List<RoleDto> Roles { get; set; } = new();

    public Guid CurrentRoleId { get; set; }

    public bool AddRoleDialogVisible { get; set; }

    public bool UpdateRoleDialogVisible { get; set; }

    public List<DataTableHeader<RoleDto>> Headers { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(RoleDto.Name)), Value = nameof(RoleDto.Name), Sortable = false },          
            new() { Text = T(nameof(RoleDto.CreationTime)), Value = nameof(RoleDto.CreationTime), Sortable = false },           
            new() { Text = T(nameof(RoleDto.ModificationTime)), Value = nameof(RoleDto.ModificationTime), Sortable = false },
            new() { Text = T(nameof(RoleDto.Creator)), Value = nameof(RoleDto.Creator), Sortable = false },
            new() { Text = T(nameof(RoleDto.Modifier)), Value = nameof(RoleDto.Modifier), Sortable = false },
            new() { Text = T("State"), Value = nameof(RoleDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetRolesAsync();
    }

    public async Task GetRolesAsync()
    {
        Loading = true;
        var reuquest = new GetRolesDto(Page, PageSize, Search, Enabled);
        var response = await RoleService.GetListAsync(reuquest);
        Roles = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddRoleDialog()
    {
        AddRoleDialogVisible = true;
    }

    public void OpenUpdateRoleDialog(RoleDto role)
    {
        CurrentRoleId = role.Id;
        UpdateRoleDialogVisible = true;
    }

    public void OpenRemoveRoleDialog(RoleDto role)
    {
        OpenConfirmDialog(async confirm =>
        {
            if (confirm) await RemoveRoleAsync(role.Id);
        }, T("Are you sure delete role data"));
    }

    public async Task RemoveRoleAsync(Guid roleId)
    {
        Loading = true;
        await RoleService.RemoveAsync(roleId);
        OpenSuccessMessage(T("Delete user data success"));
        await GetRolesAsync();
        Loading = false;
    }
}

