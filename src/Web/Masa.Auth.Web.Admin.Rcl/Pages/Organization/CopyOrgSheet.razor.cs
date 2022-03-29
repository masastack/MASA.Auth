namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class CopyOrgSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    StringNumber _step = 1;
    bool _enabled = true, _migrationStaff = false;

    List<DepartmentDto> _departments = new List<DepartmentDto> {
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
}