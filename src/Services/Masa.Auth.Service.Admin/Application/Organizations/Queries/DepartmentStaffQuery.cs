namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record DepartmentStaffQuery(Guid DepartmentId) : Query<List<StaffItemDto>>
{
    public override List<StaffItemDto> Result { get; set; } = new();
}
