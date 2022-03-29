using Masa.Auth.Contracts.Admin.Infrastructure.Models;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class Organization
{
    List<Guid> _active = new List<Guid>();
    List<DepartmentDto> _departments = new();
    bool _showAdd, _showCopy;
    DepartmentChildrenCountDto _departmentChildrenCountDto = new();
    readonly List<DataTableHeader<StaffDto>> _headers = new()
    {
        new() { Text = "员工", Value = nameof(StaffDto.Name), CellClass = "text-body neutral-lighten-1--text" },
        new() { Text = "手机号", Value = nameof(StaffDto.PhoneNumber), CellClass = "text-body3" },
        new() { Text = "邮箱", Value = nameof(StaffDto.Email), CellClass = "text-body3" },
        new() { Text = "工号", Value = nameof(StaffDto.JobNumber), CellClass = "text-body3" },
        new() { Text = "操作", Value = "Action", Sortable = false, Width = 80 }
    };
    PaginationDto<StaffDto> _paginationStaffs = new();
    AddOrUpdateDepartmentDto _addOrUpdateDepartmentDto = new();
    CopyDepartmentDto _copyDepartmentDto = new();
    GetStaffsDto _getStaffsDto = new GetStaffsDto(1, 10, "", Guid.Empty);
    DepartmentService DepartmentService => AuthCaller.DepartmentService;
    StaffService StaffService => AuthCaller.StaffService;


    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDepartmentsAsync();
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadDepartmentsAsync()
    {
        _departments = await DepartmentService.GetListAsync();
        _departmentChildrenCountDto = await DepartmentService.GetCountAsync();
    }

    private async Task LoadStaffsAsync(Guid departmentId)
    {
        _getStaffsDto.DepartmentId = departmentId;
        var data = await StaffService.GetStaffPaginationAsync(_getStaffsDto);
        _paginationStaffs = data;
    }

    private void Add(Guid parentId)
    {
        _addOrUpdateDepartmentDto = new AddOrUpdateDepartmentDto();
        _addOrUpdateDepartmentDto.ParentId = parentId;
        _showAdd = true;
    }

    private async Task DeleteAsync(Guid departmentId)
    {
        await DepartmentService.RemoveAsync(departmentId);
        await LoadDepartmentsAsync();
    }

    private async Task Update(Guid departmentId)
    {
        var department = await DepartmentService.GetAsync(departmentId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _addOrUpdateDepartmentDto = new AddOrUpdateDepartmentDto();
        _addOrUpdateDepartmentDto.Id = department.Id;
        _addOrUpdateDepartmentDto.Name = department.Name;
        _addOrUpdateDepartmentDto.Description = department.Description;
        _addOrUpdateDepartmentDto.Enabled = department.Enabled;
        _addOrUpdateDepartmentDto.ParentId = department.ParentId;
        _showAdd = true;
    }

    private async Task SubmitAsync(AddOrUpdateDepartmentDto dto)
    {
        await DepartmentService.AddOrUpdateAsync(dto);
        await LoadDepartmentsAsync();
    }

    private async Task SubmitAsync(CopyDepartmentDto dto)
    {
        await DepartmentService.AddOrUpdateAsync(dto);
        await LoadDepartmentsAsync();
    }

    private async Task Copy(Guid sourceId)
    {
        var department = await DepartmentService.GetAsync(sourceId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _copyDepartmentDto = new CopyDepartmentDto();
        _copyDepartmentDto.Name = department.Name;
        _copyDepartmentDto.Description = department.Description;
        _copyDepartmentDto.Enabled = department.Enabled;
        _copyDepartmentDto.ParentId = department.ParentId;
        _copyDepartmentDto.Staffs = department.StaffList;
        _copyDepartmentDto.Staffs = new List<StaffDto> {
            new StaffDto(Guid.NewGuid(),"谷守到","鬼谷子","1234567858","","12345678888",""),
            new StaffDto(Guid.NewGuid(),"谷首道","鬼谷子2","1234567858","","12345678888","")
        };
        _showCopy = true;
    }

    private async Task ActiveUpdated(List<DepartmentDto> activedItems)
    {
        if (!activedItems.Any())
        {
            return;
        }
        await LoadStaffsAsync(activedItems.First().Id);
    }
}

