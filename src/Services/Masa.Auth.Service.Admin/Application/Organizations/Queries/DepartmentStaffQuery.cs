namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentStaffQuery(Guid DepartmentId) : Query<List<StaffDto>>
{
    public override List<StaffDto> Result { get; set; } = new();
}
