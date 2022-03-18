using Masa.Auth.ApiGateways.Caller.Response.Organizations;
using Masa.Auth.Contracts.Admin.Subjects;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class Org
{
    List<Guid> _active = new List<Guid>();
    List<DepartmentItemResponse> _departments = new();
    readonly List<DataTableHeader<StaffDto>> _headers = new()
    {
        new() { Text = "员工", Value = nameof(StaffDto.Name) },
        new() { Text = "手机号", Value = nameof(StaffDto.PhoneNumber) },
        new() { Text = "邮箱", Value = nameof(StaffDto.Email) },
        new() { Text = "工号", Value = nameof(StaffDto.JobNumber) },
        new() { Text = "操作", Value = "Action", Sortable = false }
    };
    List<StaffDto> staffItems = new();

    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {

        await base.OnAfterRenderAsync(firstRender);
    }

    private void OpenAddDialog(Guid parentId, string parentName)
    {

    }

    private async Task AddDepartment()
    {
        //await HandleCallerAsync(res, async () =>
        //{
        //    _addOrgDialog = false;
        //    await LoadDataAsync();
        //});
    }

    private async Task ActiveUpdated(List<DepartmentItemResponse> activedItems)
    {
        //_disableDepartmentMemberBtn = false;
        //_currentDepartment = activedItems[0];
        //var res = await UserCaller.GetUsersWithDepartmentAsync(_currentDepartmentId, true);
        //HandleCaller(res, (data) =>
        //{
        //    _departmentUsers = data;
        //});
    }
}

