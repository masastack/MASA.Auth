namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentStaffQuery(Guid DepartmentId) : Query<List<StaffItem>>
{
    public override List<StaffItem> Result { get; set; } = new();
}
