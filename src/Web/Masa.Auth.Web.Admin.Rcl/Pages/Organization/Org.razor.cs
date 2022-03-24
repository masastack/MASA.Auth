namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class Org
{
    List<Guid> _active = new List<Guid>();
    List<DepartmentDto> _departments = new();
    readonly List<DataTableHeader<StaffDto>> _headers = new()
    {
        new() { Text = "员工", Value = nameof(StaffDto.Name), CellClass = "text-body neutral-lighten-1--text" },
        new() { Text = "手机号", Value = nameof(StaffDto.PhoneNumber), CellClass = "text-body3" },
        new() { Text = "邮箱", Value = nameof(StaffDto.Email), CellClass = "text-body3" },
        new() { Text = "工号", Value = nameof(StaffDto.JobNumber), CellClass = "text-body3" },
        new() { Text = "操作", Value = "Action", Sortable = false, Width = 80 }
    };
    List<StaffDto> staffs = new();

    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            _departments = new List<DepartmentDto> {
                new DepartmentDto
                {
                    Name ="MasaStack",
                    Id = Guid.NewGuid(),
                    Children = new List<DepartmentDto> {
                        new DepartmentDto { Name ="Stack业务部",Id = Guid.NewGuid()},
                        new DepartmentDto { Name ="Stack研发部",Id = Guid.NewGuid()},
                    }
                }
            };
            staffs = new List<StaffDto> {
                new StaffDto(Guid.NewGuid(),"研发部","开发","12412489",true,"新员工","https://cdn.masastack.com/stack/images/website/masa-blazor/lists/2.png","18267367890","13562763@qq.com")
            };
            StateHasChanged();
        }

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

    private async Task ActiveUpdated(List<DepartmentDto> activedItems)
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

