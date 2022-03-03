namespace MASA.Auth.Admin.Pages.Organization;

public partial class Org
{
    bool _addOrgDialog, _addDepartmentUserDialog, _disableDepartmentMemberBtn = true;
    List<Guid> _active = new List<Guid>();
    List<DepartmentItem> _departments = new();
    CreateDepartment _createDepartment = new();
    DepartmentItem _currentDepartment = new();

    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OpenAddDialog(Guid parentId, string parentName)
    {
        _createDepartment = new()
        {

        };
        _addOrgDialog = true;
    }

    private async Task AddDepartment()
    {
        //await HandleCallerAsync(res, async () =>
        //{
        //    _addOrgDialog = false;
        //    await LoadDataAsync();
        //});
    }

    private async Task ActiveUpdated(List<DepartmentItem> activedItems)
    {
        _disableDepartmentMemberBtn = false;
        _currentDepartment = activedItems[0];
        //var res = await UserCaller.GetUsersWithDepartmentAsync(_currentDepartmentId, true);
        //HandleCaller(res, (data) =>
        //{
        //    _departmentUsers = data;
        //});
    }
}

