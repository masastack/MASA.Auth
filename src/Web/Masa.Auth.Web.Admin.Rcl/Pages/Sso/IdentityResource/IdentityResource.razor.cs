namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.IdentityResource;

public partial class IdentityResource
{
    private string? _search;
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

    public List<DataTableHeader<RoleDto>> Headers { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    public IdentityResource()
    {

    }

    public async Task GetIdentityResourcesAsync()
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

    public async Task OpenRemoveRoleDialog(RoleDto role)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete role data"));
        if (confirm) await RemoveRoleAsync(role.Id);
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

