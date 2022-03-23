namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class OrgSheet
{
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
